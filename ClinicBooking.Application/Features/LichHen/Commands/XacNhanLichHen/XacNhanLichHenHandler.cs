using ClinicBooking.Application.Abstractions.Notifications;
using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Domain.Entities;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.LichHen.Commands.XacNhanLichHen;

public class XacNhanLichHenHandler : IRequestHandler<XacNhanLichHenCommand, Unit>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly INotificationService _notificationService;

    public XacNhanLichHenHandler(
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

    public async Task<Unit> Handle(XacNhanLichHenCommand request, CancellationToken cancellationToken)
    {
        var lichHen = await _db.LichHen
            .FirstOrDefaultAsync(x => x.IdLichHen == request.IdLichHen, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay lich hen.");

        if (lichHen.TrangThai != TrangThaiLichHen.ChoXacNhan)
        {
            throw new ConflictException("Chi co the xac nhan lich hen dang cho xac nhan.");
        }

        lichHen.TrangThai = TrangThaiLichHen.DaXacNhan;

        _db.LichSuLichHen.Add(new LichSuLichHen
        {
            IdLichHen = lichHen.IdLichHen,
            HanhDong = HanhDongLichSu.XacNhan,
            IdNguoiThucHien = _currentUser.IdTaiKhoan,
            NgayTao = _dateTimeProvider.UtcNow
        });

        await _db.SaveChangesAsync(cancellationToken);

        // Fire-and-forget — stub chi log.
        await _notificationService.GuiThongBaoXacNhanLichHenAsync(lichHen.IdLichHen, cancellationToken);

        return Unit.Value;
    }
}
