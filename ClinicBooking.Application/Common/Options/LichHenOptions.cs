namespace ClinicBooking.Application.Common.Options;

/// <summary>
/// Cau hinh nghiep vu cho Module 1 (Dat lich hen &amp; Hang cho).
/// Bind tu section <c>LichHen</c> trong <c>appsettings.json</c>.
/// </summary>
public class LichHenOptions
{
    public const string SectionName = "LichHen";

    /// <summary>
    /// Nguong "huy muon" tinh theo so gio truoc thoi diem kham.
    /// Neu benh nhan huy trong khoang nay, <c>BenhNhan.SoLanHuyMuon</c> se tang va
    /// <c>LichSuLichHen.DanhDauHuyMuon=true</c>.
    /// </summary>
    public int HuyMuonTruocGio { get; set; } = 24;

    /// <summary>Thoi han mot luot <c>GiuCho</c> tam thoi (phut).</summary>
    public int GiuChoThoiHanPhut { get; set; } = 15;

    /// <summary>Tien to <c>MaLichHen</c>. Mac dinh "LH".</summary>
    public string MaLichHenPrefix { get; set; } = "LH";

    /// <summary>
    /// True: khi goi benh nhan ke tiep, tu dong danh dau luot hien tai la
    /// <c>TrangThaiHangCho.HoanThanh</c> neu luot do dang <c>DangKham</c>.
    /// </summary>
    public bool TuDongHoanThanhLuotHienTai { get; set; } = true;
}
