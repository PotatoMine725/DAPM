using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ClinicBooking.Infrastructure.Services.Notifications;

/// <summary>
/// Cau hinh SMTP de gui email.
/// Bind tu appsettings.json section "Email".
/// Se duoc dung o Tuan 3 Module 4 khi them MailKit.
/// </summary>
public sealed class EmailSettings
{
    public const string SectionName = "Email";

    /// <summary>Host SMTP (vi du: smtp.gmail.com)</summary>
    public string SmtpHost { get; init; } = string.Empty;

    /// <summary>Port SMTP (587 cho TLS, 465 cho SSL)</summary>
    public int SmtpPort { get; init; } = 587;

    /// <summary>Ten hien thi cua nguoi gui (vi du: "Phong Kham ABC")</summary>
    public string TenNguoiGui { get; init; } = "ClinicBooking";

    /// <summary>Dia chi email gui di (vi du: noreply@clinic.com)</summary>
    public string DiaChiGui { get; init; } = string.Empty;

    /// <summary>Ten dang nhap SMTP</summary>
    public string TenDangNhap { get; init; } = string.Empty;

    /// <summary>Mat khau SMTP (nen dung App Password neu Gmail)</summary>
    public string MatKhau { get; init; } = string.Empty;

    /// <summary>True: dung SSL/TLS ngay tu dau (port 465). False: dung STARTTLS (port 587).</summary>
    public bool DungSsl { get; init; } = false;
}