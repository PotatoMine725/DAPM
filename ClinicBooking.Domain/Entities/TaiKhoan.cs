using ClinicBooking.Domain.Enums;

namespace ClinicBooking.Domain.Entities;

public class TaiKhoan
{
    public int IdTaiKhoan { get; set; }
    public string TenDangNhap { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SoDienThoai { get; set; } = string.Empty;
    public string MatKhau { get; set; } = string.Empty;
    public VaiTro VaiTro { get; set; }
    public bool TrangThai { get; set; }
    public DateTime? LanDangNhapCuoi { get; set; }
    public DateTime NgayTao { get; set; }

    // Navigation
    public BenhNhan? BenhNhan { get; set; }
    public BacSi? BacSi { get; set; }
    public LeTan? LeTan { get; set; }
    public ICollection<ThongBao> DanhSachThongBao { get; set; } = new List<ThongBao>();
    public ICollection<OtpLog> DanhSachOtpLog { get; set; } = new List<OtpLog>();
    public ICollection<RefreshToken> DanhSachRefreshToken { get; set; } = new List<RefreshToken>();
}
