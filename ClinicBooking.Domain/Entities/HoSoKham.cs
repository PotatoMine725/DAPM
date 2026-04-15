namespace ClinicBooking.Domain.Entities;

public class HoSoKham
{
    public int IdHoSoKham { get; set; }
    public int IdLichHen { get; set; }
    public int IdBacSi { get; set; }
    public string? ChanDoan { get; set; }
    public string? KetQuaKham { get; set; }
    public string? GhiChu { get; set; }
    public DateTime NgayKham { get; set; }
    public DateTime NgayTao { get; set; }

    // Navigation
    public LichHen LichHen { get; set; } = null!;
    public BacSi BacSi { get; set; } = null!;
    public ICollection<ToaThuoc> DanhSachToaThuoc { get; set; } = new List<ToaThuoc>();
}
