namespace ClinicBooking.Application.Common.Constants;

/// <summary>
/// Hang so phuc vu Module 1 (Dat lich hen).
/// Cac gia tri co the cau hinh qua appsettings dung <see cref="Options.LichHenOptions"/> —
/// chi dat o day nhung thong so khong can cau hinh.
/// </summary>
public static class LichHenConstants
{
    /// <summary>So lan thu lai toi da khi va cham unique index (vi du SoSlot, SoThuTu, MaLichHen).</summary>
    public const int SoLanThuLaiToiDa = 3;

    /// <summary>Kich thuoc trang mac dinh cho cac query danh sach lich hen.</summary>
    public const int KichThuocTrangMacDinh = 20;

    /// <summary>Kich thuoc trang toi da cho phep client request.</summary>
    public const int KichThuocTrangToiDa = 100;

    /// <summary>Do dai phan sequence trong <c>MaLichHen</c> (LH-yyyyMMdd-000042).</summary>
    public const int DoDaiSequenceMaLichHen = 6;
}
