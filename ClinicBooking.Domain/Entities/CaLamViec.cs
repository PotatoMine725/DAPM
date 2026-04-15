using ClinicBooking.Domain.Enums;

namespace ClinicBooking.Domain.Entities;

public class CaLamViec
{
    public int IdCaLamViec { get; set; }
    public int IdBacSi { get; set; }
    public int IdPhong { get; set; }
    public int IdChuyenKhoa { get; set; }
    public int IdDinhNghiaCa { get; set; }
    public DateOnly NgayLamViec { get; set; }
    public TimeOnly GioBatDau { get; set; }
    public TimeOnly GioKetThuc { get; set; }
    public int ThoiGianSlot { get; set; }
    public int SoSlotToiDa { get; set; }
    public int SoSlotDaDat { get; set; }
    public TrangThaiDuyetCa TrangThaiDuyet { get; set; }
    public NguonTaoCa NguonTaoCa { get; set; }
    public int? IdBacSiYeuCau { get; set; }
    public int? IdAdminDuyet { get; set; }
    public string? LyDoTuChoi { get; set; }
    public DateTime? NgayDuyet { get; set; }
    public DateTime NgayTao { get; set; }

    // Navigation
    public BacSi BacSi { get; set; } = null!;
    public Phong Phong { get; set; } = null!;
    public ChuyenKhoa ChuyenKhoa { get; set; } = null!;
    public DinhNghiaCa DinhNghiaCa { get; set; } = null!;
    public BacSi? BacSiYeuCau { get; set; }
    public TaiKhoan? AdminDuyet { get; set; }
    public ICollection<LichHen> DanhSachLichHen { get; set; } = new List<LichHen>();
    public ICollection<DonNghiPhep> DanhSachDonNghiPhep { get; set; } = new List<DonNghiPhep>();
    public ICollection<GiuCho> DanhSachGiuCho { get; set; } = new List<GiuCho>();
    public ICollection<HangCho> DanhSachHangCho { get; set; } = new List<HangCho>();
}
