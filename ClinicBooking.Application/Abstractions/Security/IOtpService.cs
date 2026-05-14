namespace ClinicBooking.Application.Abstractions.Security;

public interface IOtpService
{
    Task<string> TaoVaGuiOtpDatLichAsync(int idTaiKhoan, string soDienThoai, string? emailGui = null, CancellationToken cancellationToken = default);

    Task<bool> XacThucOtpDatLichAsync(int idTaiKhoan, string maOtp, CancellationToken cancellationToken = default);
}
