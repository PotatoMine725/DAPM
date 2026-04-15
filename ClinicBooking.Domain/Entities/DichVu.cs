namespace ClinicBooking.Domain.Entities;

public class DichVu
{
    public int IdDichVu { get; set; }
    public int IdChuyenKhoa { get; set; }
    public string TenDichVu { get; set; } = string.Empty;
    public string? MoTa { get; set; }
    public int? ThoiGianUocTinh { get; set; }
    public bool HienThi { get; set; }
    public DateTime NgayTao { get; set; }

    // Navigation
    public ChuyenKhoa ChuyenKhoa { get; set; } = null!;
    public ICollection<LichHen> DanhSachLichHen { get; set; } = new List<LichHen>();
}
