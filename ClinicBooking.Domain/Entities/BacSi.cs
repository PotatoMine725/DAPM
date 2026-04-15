using ClinicBooking.Domain.Enums;

namespace ClinicBooking.Domain.Entities;

public class BacSi
{
    public int IdBacSi { get; set; }
    public int IdTaiKhoan { get; set; }
    public int IdChuyenKhoa { get; set; }
    public string HoTen { get; set; } = string.Empty;
    public string? AnhDaiDien { get; set; }
    public string? BangCap { get; set; }
    public int? NamKinhNghiem { get; set; }
    public string? TieuSu { get; set; }
    public LoaiHopDong LoaiHopDong { get; set; }
    public TrangThaiBacSi TrangThai { get; set; }
    public DateTime NgayTao { get; set; }

    // Navigation
    public TaiKhoan TaiKhoan { get; set; } = null!;
    public ChuyenKhoa ChuyenKhoa { get; set; } = null!;
    public ICollection<CaLamViec> DanhSachCaLamViec { get; set; } = new List<CaLamViec>();
    public ICollection<LichNoiTru> DanhSachLichNoiTru { get; set; } = new List<LichNoiTru>();
    public ICollection<DonNghiPhep> DanhSachDonNghiPhep { get; set; } = new List<DonNghiPhep>();
    public ICollection<HoSoKham> DanhSachHoSoKham { get; set; } = new List<HoSoKham>();
}
