using ClinicBooking.Domain.Enums;

namespace ClinicBooking.Domain.Entities;

public class DonNghiPhep
{
    public int IdDonNghiPhep { get; set; }
    public int IdBacSi { get; set; }
    public int IdCaLamViec { get; set; }
    public LoaiNghiPhep LoaiNghiPhep { get; set; }
    public string LyDo { get; set; } = string.Empty;
    public TrangThaiDuyetDon TrangThaiDuyet { get; set; }
    public int? IdNguoiDuyet { get; set; }
    public string? LyDoTuChoi { get; set; }
    public DateTime NgayGuiDon { get; set; }
    public DateTime? NgayXuLy { get; set; }

    // Navigation
    public BacSi BacSi { get; set; } = null!;
    public CaLamViec CaLamViec { get; set; } = null!;
    public TaiKhoan? NguoiDuyet { get; set; }
}
