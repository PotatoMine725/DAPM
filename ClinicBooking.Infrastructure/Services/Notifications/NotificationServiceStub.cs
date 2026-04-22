using ClinicBooking.Application.Abstractions.Notifications;
using ClinicBooking.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace ClinicBooking.Infrastructure.Services.Notifications;

/// <summary>
/// Implementation tam thoi cua <see cref="INotificationService"/>.
/// Chi log ra console, khong gui thong bao that.
/// Se duoc thay bang <c>NotificationService</c> o Tuan 2 Module 4.
/// </summary>
public sealed class NotificationServiceStub : INotificationService
{
    private readonly ILogger<NotificationServiceStub> _logger;

    public NotificationServiceStub(ILogger<NotificationServiceStub> logger)
    {
        _logger = logger;
    }

    public Task GuiThongBaoTaoLichHenAsync(int idLichHen, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "[STUB] GuiThongBaoTaoLichHen: IdLichHen={IdLichHen}", idLichHen);
        return Task.CompletedTask;
    }

    public Task GuiThongBaoXacNhanLichHenAsync(int idLichHen, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "[STUB] GuiThongBaoXacNhanLichHen: IdLichHen={IdLichHen}", idLichHen);
        return Task.CompletedTask;
    }

    public Task GuiThongBaoHuyLichHenAsync(
        int idLichHen,
        string lyDo,
        bool doPhongKhamHuy,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "[STUB] GuiThongBaoHuyLichHen: IdLichHen={IdLichHen}, LyDo={LyDo}, DoPhongKhamHuy={DoPhongKhamHuy}",
            idLichHen, lyDo, doPhongKhamHuy);
        return Task.CompletedTask;
    }

    public Task GuiThongBaoDoiLichHenAsync(
        int idLichHenCu,
        int idLichHenMoi,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "[STUB] GuiThongBaoDoiLichHen: IdLichHenCu={IdLichHenCu}, IdLichHenMoi={IdLichHenMoi}",
            idLichHenCu, idLichHenMoi);
        return Task.CompletedTask;
    }

    public Task GuiThongBaoCheckInAsync(int idHangCho, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "[STUB] GuiThongBaoCheckIn: IdHangCho={IdHangCho}", idHangCho);
        return Task.CompletedTask;
    }

    public Task GuiThongBaoGoiBenhNhanAsync(int idHangCho, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "[STUB] GuiThongBaoGoiBenhNhan: IdHangCho={IdHangCho}", idHangCho);
        return Task.CompletedTask;
    }

    public Task GuiAsync(
        int idTaiKhoanNhan,
        LoaiThongBao loai,
        Dictionary<string, string> duLieu,
        LoaiThamChieu? loaiThamChieu = null,
        int? idThamChieu = null,
        CancellationToken ct = default)
    {
        _logger.LogInformation(
            "[STUB] Gui: IdTaiKhoanNhan={IdTaiKhoanNhan}, Loai={Loai}, LoaiThamChieu={LoaiThamChieu}, IdThamChieu={IdThamChieu}",
            idTaiKhoanNhan, loai, loaiThamChieu, idThamChieu);
        return Task.CompletedTask;
    }
}