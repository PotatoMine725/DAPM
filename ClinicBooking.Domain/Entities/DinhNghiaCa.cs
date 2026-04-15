namespace ClinicBooking.Domain.Entities;

public class DinhNghiaCa
{
    public int IdDinhNghiaCa { get; set; }
    public string TenCa { get; set; } = string.Empty;
    public TimeOnly GioBatDauMacDinh { get; set; }
    public TimeOnly GioKetThucMacDinh { get; set; }
    public string? MoTa { get; set; }
    public bool TrangThai { get; set; }

    // Navigation
    public ICollection<CaLamViec> DanhSachCaLamViec { get; set; } = new List<CaLamViec>();
    public ICollection<LichNoiTru> DanhSachLichNoiTru { get; set; } = new List<LichNoiTru>();
}
