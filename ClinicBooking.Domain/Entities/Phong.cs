namespace ClinicBooking.Domain.Entities;

public class Phong
{
    public int IdPhong { get; set; }
    public string MaPhong { get; set; } = string.Empty;
    public string TenPhong { get; set; } = string.Empty;
    public int? SucChua { get; set; }
    public string? TrangBi { get; set; }
    public bool TrangThai { get; set; }

    // Navigation
    public ICollection<CaLamViec> DanhSachCaLamViec { get; set; } = new List<CaLamViec>();
    public ICollection<LichNoiTru> DanhSachLichNoiTru { get; set; } = new List<LichNoiTru>();
}
