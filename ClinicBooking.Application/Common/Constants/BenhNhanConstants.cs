namespace ClinicBooking.Application.Common.Constants;

/// <summary>
/// Hang so phuc vu Module 3 (Benh nhan, Ho so kham, Toa thuoc).
/// Cac gia tri co the cau hinh qua appsettings dung <see cref="Options.BenhNhanOptions"/> —
/// chi dat o day nhung thong so khong can cau hinh.
/// </summary>
public static class BenhNhanConstants
{
    /// <summary>Nguong toi da so lan huy lich hen trong mot thang. Neu vuot qua, benh nhan bi han che.</summary>
    public const int NgueongSoLanHuyMuonTrongThang = 3;

    /// <summary>Chu ky reset so lan huy muon (theo thang).</summary>
    public const int ChuKyResetThang = 1;
}
