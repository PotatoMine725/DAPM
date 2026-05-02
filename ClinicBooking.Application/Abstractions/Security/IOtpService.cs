namespace ClinicBooking.Application.Abstractions.Security;

public interface IOtpService
{
    Task<string> TaoVaGuiOtpDatLichAsync(int idTaiKhoan, string soDienThoai, CancellationToken cancellationToken = default);

    Task<bool> XacThucOtpDatLichAsync(int idTaiKhoan, string maOtp, CancellationToken cancellationToken = default);
}
