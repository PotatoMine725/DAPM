using ClinicBooking.Domain.Enums;

namespace ClinicBooking.Domain.Entities;

public class ThongBao
{
    public int IdThongBao { get; set; }
    public int IdTaiKhoan { get; set; }
    public int IdMau { get; set; }
    public KenhGui KenhGui { get; set; }
    public string TieuDe { get; set; } = string.Empty;
    public string NoiDung { get; set; } = string.Empty;
    public int? IdThamChieu { get; set; }
    public LoaiThamChieu? LoaiThamChieu { get; set; }
    public bool DaDoc { get; set; }
    public DateTime? NgayGui { get; set; }

    // Navigation
    public TaiKhoan TaiKhoan { get; set; } = null!;
    public MauThongBao MauThongBao { get; set; } = null!;
}
