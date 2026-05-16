namespace ClinicBooking.Application.Common.Options;

/// <summary>
/// Cấu hình SMTP cho gửi email.
/// </summary>
public class EmailSettings
{
    public const string SectionName = "Email";

    /// <summary>
    /// SMTP server hostname.
    /// </summary>
    public string SmtpHost { get; set; } = string.Empty;

    /// <summary>
    /// SMTP port.
    /// </summary>
    public int SmtpPort { get; set; } = 587;

    /// <summary>
    /// Username for SMTP authentication.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Password for SMTP authentication.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Enable TLS/SSL.
    /// </summary>
    public bool StartTls { get; set; } = true;

    /// <summary>
    /// From email address.
    /// </summary>
    public string FromAddress { get; set; } = string.Empty;

    /// <summary>
    /// From display name.
    /// </summary>
    public string FromName { get; set; } = string.Empty;
}
