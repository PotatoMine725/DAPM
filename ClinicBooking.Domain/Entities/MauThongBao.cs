using ClinicBooking.Domain.Enums;

namespace ClinicBooking.Domain.Entities;

public class MauThongBao
{
    public int IdMau { get; set; }
    public LoaiThongBao LoaiThongBao { get; set; }
    public string TieuDeMau { get; set; } = string.Empty;
    public string NoiDungMau { get; set; } = string.Empty;
    public KenhGui KenhGui { get; set; }

    // Navigation
    public ICollection<ThongBao> DanhSachThongBao { get; set; } = new List<ThongBao>();
}
