using ClinicBooking.Application.Abstractions.Notifications;
using ClinicBooking.Application.Abstractions.Persistence;
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
    private readonly LichHenOptions _options;

    public GoiBenhNhanKeTiepHandler(
        IAppDbContext db,
        INotificationService notificationService,
        IOptions<LichHenOptions> options)
    {
        _db = db;
        _notificationService = notificationService;
        _options = options.Value;
    }

    public async Task<HangChoResponse> Handle(GoiBenhNhanKeTiepCommand request, CancellationToken cancellationToken)
    {
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
}
