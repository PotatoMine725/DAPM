using ClinicBooking.Domain.Enums;

namespace ClinicBooking.Domain.Entities;

public class OtpLog
{
    public int IdOtpLog { get; set; }
    public int IdTaiKhoan { get; set; }
    public string MaOtp { get; set; } = string.Empty;
    public MucDichOtp MucDich { get; set; }
    public DateTime GioHetHan { get; set; }
    public bool DaSuDung { get; set; }
    public int SoLanThu { get; set; }
    public string? IdPhien { get; set; }
    public string? DiaChiIp { get; set; }
    public DateTime NgayTao { get; set; }

    // Navigation
    public TaiKhoan TaiKhoan { get; set; } = null!;
}
