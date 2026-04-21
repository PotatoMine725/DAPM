namespace ClinicBooking.Infrastructure.Persistence;

/// <summary>
/// Cau hinh cho viec seed tai khoan admin + tai khoan dev khi app khoi dong.
/// Binding voi section "Admin" trong appsettings.
/// </summary>
public class AdminSeederSettings
{
    public const string SectionName = "Admin";

    /// <summary>
    /// Mat khau mac dinh dung cho lan fix hash admin dau tien.
    /// KHONG bo vao appsettings.json production — chi de trong appsettings.Development.json,
    /// User Secrets, hoac bien moi truong.
    /// </summary>
    public string? MatKhauMacDinh { get; set; }

    /// <summary>
    /// Cau hinh seed tai khoan fixture (le_tan, bac_si, benh_nhan) cho moi truong dev.
    /// Null / rong = bo qua, khong seed.
    /// </summary>
    public DevFixtureSettings? DevFixture { get; set; }
}

/// <summary>
/// Tai khoan dev seed boi DatabaseSeeder de team test cac luong ngoai admin.
/// CHI dung o moi truong Development.
/// </summary>
public class DevFixtureSettings
{
    /// <summary>Bat/tat toan bo dev fixture. Mac dinh false de khong seed o prod.</summary>
    public bool Enabled { get; set; } = false;

    /// <summary>
    /// Mat khau dung chung cho cac tai khoan fixture (le_tan, bac_si mau).
    /// Neu khong dat, se fallback ve AdminSeederSettings.MatKhauMacDinh.
    /// </summary>
    public string? MatKhauChung { get; set; }

    /// <summary>Tai khoan le_tan mau. Null = bo qua.</summary>
    public FixtureTaiKhoan? LeTan { get; set; }
}

public class FixtureTaiKhoan
{
    public string TenDangNhap { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SoDienThoai { get; set; } = string.Empty;
    public string HoTen { get; set; } = string.Empty;
}
