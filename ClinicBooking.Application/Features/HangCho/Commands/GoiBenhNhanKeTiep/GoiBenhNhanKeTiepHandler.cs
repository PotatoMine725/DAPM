using ClinicBooking.Application.Abstractions.Notifications;
using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Common.Options;
using ClinicBooking.Application.Features.HangCho.Dtos;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ClinicBooking.Application.Features.HangCho.Commands.GoiBenhNhanKeTiep;

public class GoiBenhNhanKeTiepHandler : IRequestHandler<GoiBenhNhanKeTiepCommand, HangChoResponse>
{
    private readonly IAppDbContext _db;
    private readonly INotificationService _notificationService;
    private readonly ICurrentUserService _currentUser;
    private readonly LichHenOptions _options;

    public GoiBenhNhanKeTiepHandler(
        IAppDbContext db,
        INotificationService notificationService,
        ICurrentUserService currentUser,
        IOptions<LichHenOptions> options)
    {
        _db = db;
        _notificationService = notificationService;
        _currentUser = currentUser;
        _options = options.Value;
    }

    public async Task<HangChoResponse> Handle(GoiBenhNhanKeTiepCommand request, CancellationToken cancellationToken)
    {
        await KiemTraQuyenGoiAsync(request.IdCaLamViec, cancellationToken);

        // Auto-complete luot hien tai neu option bat.
        if (_options.TuDongHoanThanhLuotHienTai)
        {
            var luotDangKham = await _db.HangCho
                .Include(x => x.LichHen)
                .Where(x => x.IdCaLamViec == request.IdCaLamViec && x.TrangThai == TrangThaiHangCho.DangKham)
                .OrderBy(x => x.SoThuTu)
                .FirstOrDefaultAsync(cancellationToken);

            if (luotDangKham is not null)
            {
                luotDangKham.TrangThai = TrangThaiHangCho.HoanThanh;
                luotDangKham.LichHen.TrangThai = TrangThaiLichHen.HoanThanh;
            }
        }

        var keTiep = await _db.HangCho
            .Include(x => x.LichHen).ThenInclude(l => l.BenhNhan)
            .Where(x => x.IdCaLamViec == request.IdCaLamViec && x.TrangThai == TrangThaiHangCho.ChoKham)
            .OrderBy(x => x.SoThuTu)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("Khong con benh nhan nao trong hang cho.");

        keTiep.TrangThai = TrangThaiHangCho.DangKham;
        keTiep.LichHen.TrangThai = TrangThaiLichHen.DangKham;

        await _db.SaveChangesAsync(cancellationToken);

        await _notificationService.GuiThongBaoGoiBenhNhanAsync(keTiep.IdHangCho, cancellationToken);

        return new HangChoResponse(
            keTiep.IdHangCho,
            keTiep.IdCaLamViec,
            keTiep.IdLichHen,
            keTiep.LichHen.MaLichHen,
            keTiep.LichHen.IdBenhNhan,
            keTiep.LichHen.BenhNhan.HoTen,
            keTiep.SoThuTu,
            keTiep.TrangThai,
            keTiep.NgayCheckIn);
    }

    private async Task KiemTraQuyenGoiAsync(int idCaLamViec, CancellationToken cancellationToken)
    {
        // Le tan/admin khong rang buoc ownership — ho dieu phoi quay.
        // Bac si chi duoc goi benh nhan trong ca ma minh phu trach.
        if (_currentUser.VaiTro != VaiTro.BacSi)
        {
            return;
        }

        var idTaiKhoan = _currentUser.IdTaiKhoan
            ?? throw new ForbiddenException("Khong xac dinh duoc nguoi dung hien tai.");

        var idBacSiCa = await _db.CaLamViec
            .AsNoTracking()
            .Where(x => x.IdCaLamViec == idCaLamViec)
            .Select(x => (int?)x.IdBacSi)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("Khong tim thay ca lam viec.");

        var laBacSiCuaCa = await _db.BacSi
            .AsNoTracking()
            .AnyAsync(bs => bs.IdBacSi == idBacSiCa && bs.IdTaiKhoan == idTaiKhoan, cancellationToken);

        if (!laBacSiCuaCa)
        {
            throw new ForbiddenException("Ban khong phu trach ca lam viec nay.");
        }
    }
}
