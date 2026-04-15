using ClinicBooking.Domain.Enums;

namespace ClinicBooking.Domain.Entities;

public class LichSuLichHen
{
    public int IdLichSu { get; set; }
    public int IdLichHen { get; set; }
    public HanhDongLichSu HanhDong { get; set; }
    public int? IdNguoiThucHien { get; set; }
    public string? LyDo { get; set; }
    public int? IdLichHenTruoc { get; set; }
    public bool DanhDauHuyMuon { get; set; }
    public DateTime NgayTao { get; set; }

    // Navigation
    public LichHen LichHen { get; set; } = null!;
    public TaiKhoan? NguoiThucHien { get; set; }
    public LichHen? LichHenTruoc { get; set; }
}
