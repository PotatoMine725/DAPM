using ClinicBooking.Application.Abstractions.Notifications;
using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Constants;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.HangCho.Dtos;
using ClinicBooking.Domain.Enums;
using HangChoEntity = ClinicBooking.Domain.Entities.HangCho;
using LichSuLichHenEntity = ClinicBooking.Domain.Entities.LichSuLichHen;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.LichHen.Commands.CheckInLichHen;

public class CheckInLichHenHandler : IRequestHandler<CheckInLichHenCommand, HangChoResponse>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly INotificationService _notificationService;

    public CheckInLichHenHandler(
        IAppDbContext db,
        ICurrentUserService currentUser,
        IDateTimeProvider dateTimeProvider,
        INotificationService notificationService)
    {
        _db = db;
        _currentUser = currentUser;
        _dateTimeProvider = dateTimeProvider;
        _notificationService = notificationService;
    }

    public async Task<HangChoResponse> Handle(CheckInLichHenCommand request, CancellationToken cancellationToken)
    {
        var lichHen = await _db.LichHen
            .Include(x => x.BenhNhan)
            .FirstOrDefaultAsync(x => x.IdLichHen == request.IdLichHen, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay lich hen.");

        if (lichHen.TrangThai != TrangThaiLichHen.DaXacNhan)
        {
            throw new ConflictException("Chi co the check-in lich da xac nhan.");
        }

        var daCheckIn = await _db.HangCho
            .AnyAsync(x => x.IdLichHen == lichHen.IdLichHen, cancellationToken);
        if (daCheckIn)
        {
            throw new ConflictException("Lich hen da check-in truoc do.");
        }

        // Retry toi da SoLanThuLaiToiDa lan khi va cham unique index (IdCaLamViec, SoThuTu).
        for (var thu = 0; thu < LichHenConstants.SoLanThuLaiToiDa; thu++)
        {
            var soThuTuMax = await _db.HangCho
                .Where(x => x.IdCaLamViec == lichHen.IdCaLamViec)
                .Select(x => (int?)x.SoThuTu)
                .MaxAsync(cancellationToken) ?? 0;

            var hangCho = new HangChoEntity
            {
                IdCaLamViec = lichHen.IdCaLamViec,
                IdLichHen = lichHen.IdLichHen,
                SoThuTu = soThuTuMax + 1,
                TrangThai = TrangThaiHangCho.ChoKham,
                NgayCheckIn = _dateTimeProvider.UtcNow
            };

            _db.HangCho.Add(hangCho);

            _db.LichSuLichHen.Add(new LichSuLichHenEntity
            {
                IdLichHen = lichHen.IdLichHen,
                HanhDong = HanhDongLichSu.CheckIn,
                IdNguoiThucHien = _currentUser.IdTaiKhoan,
                NgayTao = _dateTimeProvider.UtcNow
            });

            try
            {
                await _db.SaveChangesAsync(cancellationToken);

                await _notificationService.GuiThongBaoCheckInAsync(hangCho.IdHangCho, cancellationToken);

                return new HangChoResponse(
                    hangCho.IdHangCho,
                    hangCho.IdCaLamViec,
                    hangCho.IdLichHen,
                    lichHen.MaLichHen,
                    lichHen.IdBenhNhan,
                    lichHen.BenhNhan.HoTen,
                    hangCho.SoThuTu,
                    hangCho.TrangThai,
                    hangCho.NgayCheckIn);
            }
            catch (DbUpdateException)
            {
                // Xoa ban ghi pending ra khoi change tracker roi thu lai.
                _db.HangCho.Remove(hangCho);
                // LichSu insert cung se bi xoa khi handler retry tao moi o vong lap tiep theo
                // (trong thuc te DbContext nen scoped-per-request, mot lan SaveChanges fail
                // khong dam bao transaction tiep theo sach — Wave 4 can review voi transaction scope).
            }
        }

        throw new ConflictException("Khong the chiem so thu tu check-in sau nhieu lan thu. Vui long thu lai.");
    }
}
