using ClinicBooking.Domain.Enums;

namespace ClinicBooking.Domain.Entities;

public class LichHen
{
    public int IdLichHen { get; set; }
    public string MaLichHen { get; set; } = string.Empty;
    public int IdBenhNhan { get; set; }
    public int IdCaLamViec { get; set; }
    public int IdDichVu { get; set; }
    public int SoSlot { get; set; }
    public HinhThucDat HinhThucDat { get; set; }
    public string? BacSiMongMuonNote { get; set; }
    public int? IdBacSiMongMuon { get; set; }
    public string? TrieuChung { get; set; }
    public TrangThaiLichHen TrangThai { get; set; }
    public DateTime NgayTao { get; set; }

    /// <summary>
    /// Concurrency token — EF Core tu quan ly.
    /// Bao ve cac race condition khi nhieu request dong thoi huy/doi lich cung 1 lich hen.
    /// </summary>
    public byte[] RowVersion { get; set; } = [];

    // Navigation
    public BenhNhan BenhNhan { get; set; } = null!;
    public CaLamViec CaLamViec { get; set; } = null!;
    public DichVu DichVu { get; set; } = null!;
    public BacSi? BacSiMongMuon { get; set; }
    public ICollection<LichSuLichHen> DanhSachLichSu { get; set; } = new List<LichSuLichHen>();
    public HangCho? HangCho { get; set; }
    public HoSoKham? HoSoKham { get; set; }
}
