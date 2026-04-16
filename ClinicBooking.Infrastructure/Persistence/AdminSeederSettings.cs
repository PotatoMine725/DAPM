namespace ClinicBooking.Infrastructure.Persistence;

/// <summary>
/// Cau hinh cho viec seed tai khoan admin khi app khoi dong.
/// Binding voi section "Admin" trong appsettings.
/// </summary>
public class AdminSeederSettings
{
    public const string SectionName = "Admin";

    /// <summary>
    /// Mat khau mac dinh dung cho lan fix hash admin dau tien.
    /// KHONG bo vao appsettings.json — chi de trong appsettings.Development.json,
    /// User Secrets, hoac bien moi truong o moi truong production.
    /// </summary>
    public string? MatKhauMacDinh { get; set; }
}
