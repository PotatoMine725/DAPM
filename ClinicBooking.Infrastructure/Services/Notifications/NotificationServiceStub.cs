using ClinicBooking.Application.Abstractions.Notifications;
using Microsoft.Extensions.Logging;

namespace ClinicBooking.Infrastructure.Services.Notifications;

/// <summary>
/// Stub tam thoi cho <see cref="INotificationService"/>.
/// Chi log ra console — KHONG ghi vao bang <c>ThongBao</c> (thuoc Module 4).
/// TODO: Thay the bang implementation that cua Module 4 khi code duoc day len.
/// </summary>
public class NotificationServiceStub : INotificationService
{
    private readonly ILogger<NotificationServiceStub> _logger;

    public NotificationServiceStub(ILogger<NotificationServiceStub> logger)
    {
        _logger = logger;
    }

    public Task GuiThongBaoTaoLichHenAsync(int idLichHen, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[Stub] Gui thong bao TAO LICH HEN cho LichHen #{IdLichHen}", idLichHen);
        return Task.CompletedTask;
    }

    public Task GuiThongBaoXacNhanLichHenAsync(int idLichHen, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[Stub] Gui thong bao XAC NHAN LICH HEN cho LichHen #{IdLichHen}", idLichHen);
        return Task.CompletedTask;
    }

    public Task GuiThongBaoHuyLichHenAsync(
        int idLichHen, string lyDo, bool doPhongKhamHuy, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "[Stub] Gui thong bao HUY LICH HEN cho LichHen #{IdLichHen}. Ly do: {LyDo}. Phong kham huy: {DoPhongKhamHuy}",
            idLichHen, lyDo, doPhongKhamHuy);
        return Task.CompletedTask;
    }

    public Task GuiThongBaoDoiLichHenAsync(
        int idLichHenCu, int idLichHenMoi, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "[Stub] Gui thong bao DOI LICH HEN: LichHen cu #{IdLichHenCu} -> moi #{IdLichHenMoi}",
            idLichHenCu, idLichHenMoi);
        return Task.CompletedTask;
    }

    public Task GuiThongBaoCheckInAsync(int idHangCho, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[Stub] Gui thong bao CHECK-IN cho HangCho #{IdHangCho}", idHangCho);
        return Task.CompletedTask;
    }

    public Task GuiThongBaoGoiBenhNhanAsync(int idHangCho, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[Stub] Gui thong bao GOI BENH NHAN cho HangCho #{IdHangCho}", idHangCho);
        return Task.CompletedTask;
    }
}
