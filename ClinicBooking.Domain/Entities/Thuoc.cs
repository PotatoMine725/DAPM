namespace ClinicBooking.Domain.Entities;

public class Thuoc
{
    public int IdThuoc { get; set; }
    public string TenThuoc { get; set; } = string.Empty;
    public string? HoatChat { get; set; }
    public string? DonVi { get; set; }
    public string? GhiChu { get; set; }

    // Navigation
    public ICollection<ToaThuoc> DanhSachToaThuoc { get; set; } = new List<ToaThuoc>();
}
