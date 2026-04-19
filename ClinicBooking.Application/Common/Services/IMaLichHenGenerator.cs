namespace ClinicBooking.Application.Common.Services;

/// <summary>
/// Sinh <c>MaLichHen</c> theo format <c>LH-yyyyMMdd-{seq6}</c> (vi du <c>LH-20260416-000042</c>).
/// Implementation o Infrastructure dung <c>MAX(MaLichHen)</c> parse sequence + 1, retry neu trung.
/// </summary>
public interface IMaLichHenGenerator
{
    /// <summary>
    /// Sinh ma lich hen moi cho ngay chi dinh. Tham so <paramref name="ngay"/> thuong la ngay server UTC
    /// chuyen ve timezone noi dia — caller chiu trach nhiem chuyen timezone truoc khi goi.
    /// </summary>
    Task<string> SinhMaLichHenAsync(DateOnly ngay, CancellationToken cancellationToken = default);
}
