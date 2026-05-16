using ClinicBooking.Application.Abstractions.Notifications;
using ClinicBooking.Application.Common.Options;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace ClinicBooking.Infrastructure.Services.Notifications;

/// <summary>
/// Implementation gửi email SMTP dùng MailKit.
/// Module 4 - Thông báo.
/// </summary>
public sealed class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly EmailSettings _settings;

    public EmailService(
        ILogger<EmailService> logger,
        IOptions<EmailSettings> settings)
    {
        _logger = logger;
        _settings = settings.Value;
    }

    public async Task GuiEmailAsync(
        string to,
        string subject,
        string body,
        bool isHtml = false,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_settings.FromName, _settings.FromAddress));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;

            email.Body = new TextPart(isHtml ? TextFormat.Html : TextFormat.Plain)
            {
                Text = body
            };

            using var smtp = new SmtpClient();
            
            // Connect to SMTP server
            await smtp.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, 
                _settings.StartTls ? MailKit.Security.SecureSocketOptions.StartTls : 
                MailKit.Security.SecureSocketOptions.None, cancellationToken);

            // Authenticate if credentials provided
            if (!string.IsNullOrEmpty(_settings.Username) && !string.IsNullOrEmpty(_settings.Password))
            {
                await smtp.AuthenticateAsync(_settings.Username, _settings.Password, cancellationToken);
            }

            await smtp.SendAsync(email, cancellationToken);
            await smtp.DisconnectAsync(true, cancellationToken);

            _logger.LogInformation(
                "[EmailService] Da gui email thanh cong den {To} voi subject {Subject}",
                to, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "[EmailService] Loi khi gui email den {To} voi subject {Subject}",
                to, subject);
            
            // Không throw exception để không làm hỏng flow chính
            // NotificationService sẽ xử lý retry hoặc log phù hợp
        }
    }

    public async Task GuiEmailTemplateAsync(
        string to,
        string subject,
        string templatePath,
        object model,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Đọc template file
            var templateContent = await File.ReadAllTextAsync(templatePath, cancellationToken);
            
            // Simple template replacement - có thể nâng cấp dùng RazorEngine sau
            var body = ReplaceTemplatePlaceholders(templateContent, model);
            
            await GuiEmailAsync(to, subject, body, true, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "[EmailService] Loi khi gui email template {TemplatePath} den {To}",
                templatePath, to);
        }
    }

    private static string ReplaceTemplatePlaceholders(string template, object model)
    {
        // Simple placeholder replacement: {{PropertyName}}
        var properties = model.GetType().GetProperties();
        var result = template;

        foreach (var prop in properties)
        {
            var value = prop.GetValue(model)?.ToString() ?? string.Empty;
            var placeholder = $"{{{{{prop.Name}}}}}";
            result = result.Replace(placeholder, value);
        }

        return result;
    }
}
