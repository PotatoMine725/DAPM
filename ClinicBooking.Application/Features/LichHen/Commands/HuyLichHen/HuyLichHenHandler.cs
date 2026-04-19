using ClinicBooking.Application.Abstractions.Notifications;
using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Abstractions.Scheduling;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Common.Options;
using ClinicBooking.Domain.Entities;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ClinicBooking.Application.Features.LichHen.Commands.HuyLichHen;

public class HuyLichHenHandler : IRequestHandler<HuyLichHenCommand, Unit>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ICaLamViecQueryService _caLamViecQueryService;
    private readonly INotificationService _notificationService;
    private readonly LichHenOptions _options;

    public HuyLichHenHandler(
        IAppDbContext db,
        ICurrentUserService currentUser,
        IDateTimeProvider dateTimeProvider,
        ICaLamViecQueryService caLamViecQueryService,
        INotificationService notificationService,
        IOptions<LichHenOptions> options)
    {
        _db = db;
        _currentUser = currentUser;
        _dateTimeProvider = dateTimeProvider;
        _caLamViecQueryService = caLamViecQueryService;
        _notificationService = notificationService;
        _options = options.Value;
    }

    public async Task<Unit> Handle(HuyLichHenCommand request, CancellationToken cancellationToken)
    {
        var lichHen = await _db.LichHen
            .Include(x => x.CaLamViec)
            .FirstOrDefaultAsync(x => x.IdLichHen == request.IdLichHen, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay lich hen.");

        // Auth: benh_nhan chi huy duoc lich cua minh; le_tan/admin khong han che.
        var vaiTro = _currentUser.VaiTro
            ?? throw new ForbiddenException("Khong xac dinh duoc vai tro nguoi dung.");

        var doPhongKhamHuy = vaiTro is VaiTro.LeTan or VaiTro.Admin;

        if (vaiTro == VaiTro.BenhNhan)
        {
            var idTaiKhoan = _currentUser.IdTaiKhoan
                ?? throw new ForbiddenException("Khong xac dinh duoc nguoi dung hien tai.");

            var laChuSoHuu = await _db.BenhNhan
                .AsNoTracking()
                .AnyAsync(bn => bn.IdBenhNhan == lichHen.IdBenhNhan && bn.IdTaiKhoan == idTaiKhoan, cancellationToken);
            if (!laChuSoHuu)
            {
                throw new ForbiddenException("Ban khong co quyen huy lich hen nay.");
            }
        }
        else if (vaiTro == VaiTro.BacSi)
        {
            throw new ForbiddenException("Bac si khong co quyen huy lich hen.");
        }

        // Da huy hoac da dien ra -> khong huy lai.
        if (lichHen.TrangThai is TrangThaiLichHen.HuyBenhNhan
            or TrangThaiLichHen.HuyPhongKham
            or TrangThaiLichHen.HoanThanh
            or TrangThaiLichHen.KhongDen)
        {
            throw new ConflictException("Lich hen nay khong the huy o trang thai hien tai.");
        }

        // Xac dinh co phai "huy muon" khong: so thoi diem hien tai voi thoi diem bat dau ca.
        var thoiDiemKham = lichHen.CaLamViec.NgayLamViec.ToDateTime(lichHen.CaLamViec.GioBatDau, DateTimeKind.Utc);
        var now = _dateTimeProvider.UtcNow;
        var khoangCachGio = (thoiDiemKham - now).TotalHours;
        var huyMuon = !doPhongKhamHuy && khoangCachGio < _options.HuyMuonTruocGio;

        lichHen.TrangThai = doPhongKhamHuy
            ? TrangThaiLichHen.HuyPhongKham
            : TrangThaiLichHen.HuyBenhNhan;

        _db.LichSuLichHen.Add(new LichSuLichHen
        {
            IdLichHen = lichHen.IdLichHen,
            HanhDong = doPhongKhamHuy ? HanhDongLichSu.HuyPhongKham : HanhDongLichSu.HuyBenhNhan,
            IdNguoiThucHien = _currentUser.IdTaiKhoan,
            LyDo = request.LyDo,
            DanhDauHuyMuon = huyMuon,
            NgayTao = now
        });

        if (huyMuon)
        {
            // Cross-module write: BenhNhan.SoLanHuyMuon thuoc Module 3. Coordination point — notify Module 3 owner.
            var benhNhan = await _db.BenhNhan
                .FirstOrDefaultAsync(x => x.IdBenhNhan == lichHen.IdBenhNhan, cancellationToken);
            if (benhNhan is not null)
            {
                benhNhan.SoLanHuyMuon += 1;
            }
        }

        await _db.SaveChangesAsync(cancellationToken);

        // Best-effort tra slot ve — neu fail khong roll back command.
        try
        {
            await _caLamViecQueryService.IncrementSoSlotDaDatAsync(lichHen.IdCaLamViec, -1, cancellationToken);
        }
        catch
        {
            // Reconciliation job (Wave 4) se phat hien drift.
        }

        await _notificationService.GuiThongBaoHuyLichHenAsync(
            lichHen.IdLichHen, request.LyDo, doPhongKhamHuy, cancellationToken);

        return Unit.Value;
    }
}
