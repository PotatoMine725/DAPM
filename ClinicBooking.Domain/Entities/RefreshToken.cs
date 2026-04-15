namespace ClinicBooking.Domain.Entities;

public class RefreshToken
{
    public int IdRefreshToken { get; set; }
    public int IdTaiKhoan { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime HetHan { get; set; }
    public bool DaThuHoi { get; set; }
    public DateTime? NgayThuHoi { get; set; }
    public string? LyDoThuHoi { get; set; }
    public string? ThayTheBangToken { get; set; }
    public DateTime NgayTao { get; set; }

    // Navigation
    public TaiKhoan TaiKhoan { get; set; } = null!;
}
