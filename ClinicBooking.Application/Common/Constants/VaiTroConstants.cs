using ClinicBooking.Domain.Enums;

namespace ClinicBooking.Application.Common.Constants;

/// <summary>
/// Chuoi role su dung trong [Authorize(Roles = "...")] va JWT claim.
/// Phai dong bo voi quy uoc tai CLAUDE.md muc 5.
/// </summary>
public static class VaiTroConstants
{
    public const string Admin = "admin";
    public const string LeTan = "le_tan";
    public const string BacSi = "bac_si";
    public const string BenhNhan = "benh_nhan";

    public static string ToRoleClaim(this VaiTro vaiTro) => vaiTro switch
    {
        VaiTro.Admin => Admin,
        VaiTro.LeTan => LeTan,
        VaiTro.BacSi => BacSi,
        VaiTro.BenhNhan => BenhNhan,
        _ => throw new ArgumentOutOfRangeException(nameof(vaiTro), vaiTro, "Vai tro khong hop le")
    };
}
