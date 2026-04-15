using ClinicBooking.Domain.Enums;

namespace ClinicBooking.Domain.Entities;

public class BenhNhan
{
    public int IdBenhNhan { get; set; }
    public int IdTaiKhoan { get; set; }
    public string HoTen { get; set; } = string.Empty;
    public string? Cccd { get; set; }
    public DateOnly? NgaySinh { get; set; }
    public GioiTinh? GioiTinh { get; set; }
    public string? DiaChi { get; set; }
    public int SoLanHuyMuon { get; set; }
    public bool BiHanChe { get; set; }
    public DateTime? NgayHetHanChe { get; set; }
    public DateTime NgayTao { get; set; }

    // Navigation
    public TaiKhoan TaiKhoan { get; set; } = null!;
    public ICollection<LichHen> DanhSachLichHen { get; set; } = new List<LichHen>();
    public ICollection<GiuCho> DanhSachGiuCho { get; set; } = new List<GiuCho>();
}
