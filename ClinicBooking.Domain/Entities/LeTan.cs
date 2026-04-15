namespace ClinicBooking.Domain.Entities;

public class LeTan
{
    public int IdLeTan { get; set; }
    public int IdTaiKhoan { get; set; }
    public string HoTen { get; set; } = string.Empty;
    public DateTime NgayTao { get; set; }

    // Navigation
    public TaiKhoan TaiKhoan { get; set; } = null!;
}
