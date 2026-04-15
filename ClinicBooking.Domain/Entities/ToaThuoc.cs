namespace ClinicBooking.Domain.Entities;

public class ToaThuoc
{
    public int IdToaThuoc { get; set; }
    public int IdHoSoKham { get; set; }
    public int IdThuoc { get; set; }
    public string? LieuLuong { get; set; }
    public string? CachDung { get; set; }
    public int? SoNgayDung { get; set; }
    public string? GhiChu { get; set; }

    // Navigation
    public HoSoKham HoSoKham { get; set; } = null!;
    public Thuoc Thuoc { get; set; } = null!;
}
