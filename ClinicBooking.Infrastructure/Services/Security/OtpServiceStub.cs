using ClinicBooking.Application.Abstractions.Security;
using Microsoft.Extensions.Logging;

namespace ClinicBooking.Infrastructure.Services.Security;

public sealed class OtpServiceStub : IOtpService
{
    private readonly ILogger<OtpServiceStub> _logger;

    public OtpServiceStub(ILogger<OtpServiceStub> logger)
    {
        _logger = logger;
    }

    public Task<string> TaoVaGuiOtpDatLichAsync(int idTaiKhoan, string soDienThoai, CancellationToken cancellationToken = default)
    {
        var otp = "123456";
        _logger.LogInformation("[Stub] Tao OTP dat lich cho TaiKhoan #{IdTaiKhoan}, SDT {SoDienThoai}: {Otp}", idTaiKhoan, soDienThoai, otp);
        return Task.FromResult(otp);
    }

    public Task<bool> XacThucOtpDatLichAsync(int idTaiKhoan, string maOtp, CancellationToken cancellationToken = default)
    {
        var ok = maOtp == "123456";
        _logger.LogInformation("[Stub] Xac thuc OTP dat lich cho TaiKhoan #{IdTaiKhoan}: {Ok}", idTaiKhoan, ok);
        return Task.FromResult(ok);
    }
}
