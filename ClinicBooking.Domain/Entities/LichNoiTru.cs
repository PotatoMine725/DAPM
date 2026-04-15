namespace ClinicBooking.Domain.Entities;

public class LichNoiTru
{
    public int IdLichNoiTru { get; set; }
    public int IdBacSi { get; set; }
    public int IdPhong { get; set; }
    public int IdDinhNghiaCa { get; set; }
    public int NgayTrongTuan { get; set; }
    public DateOnly NgayApDung { get; set; }
    public DateOnly? NgayKetThuc { get; set; }
    public bool TrangThai { get; set; }

    // Navigation
    public BacSi BacSi { get; set; } = null!;
    public Phong Phong { get; set; } = null!;
    public DinhNghiaCa DinhNghiaCa { get; set; } = null!;
}
