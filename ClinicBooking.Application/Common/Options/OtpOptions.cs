namespace ClinicBooking.Application.Common.Options;

public sealed class OtpOptions
{
    public const string SectionName = "Otp";

    public int ThoiHanPhut { get; set; } = 5;
    public int SoLanNhapSaiToiDa { get; set; } = 3;
    public bool BatBuocChoDatLich { get; set; } = true;
}
