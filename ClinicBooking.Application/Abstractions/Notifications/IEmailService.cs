namespace ClinicBooking.Application.Abstractions.Notifications;

/// <summary>
/// Service gửi email SMTP cho hệ thống thông báo.
/// Implement bởi Module 4 dùng MailKit.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Gửi email đơn giản.
    /// </summary>
    Task GuiEmailAsync(
        string to,
        string subject,
        string body,
        bool isHtml = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gửi email với template.
    /// </summary>
    Task GuiEmailTemplateAsync(
        string to,
        string subject,
        string templatePath,
        object model,
        CancellationToken cancellationToken = default);
}
