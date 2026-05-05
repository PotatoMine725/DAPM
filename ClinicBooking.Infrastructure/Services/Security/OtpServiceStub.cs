using System.Security.Cryptography;
using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Options;
using ClinicBooking.Domain.Entities;
using ClinicBooking.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ClinicBooking.Infrastructure.Services.Security;

public sealed class OtpServiceStub : IOtpService
{
    private readonly IAppDbContext _db;
    private readonly OtpOptions _options;
    private readonly ILogger<OtpServiceStub> _logger;

    public OtpServiceStub(
        IAppDbContext db,
        IOptions<OtpOptions> options,
        ILogger<OtpServiceStub> logger)
    {
        _db = db;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<string> TaoVaGuiOtpDatLichAsync(int idTaiKhoan, string soDienThoai, CancellationToken cancellationToken = default)
    {
        var otp = TaoMaOtp();
        var now = DateTime.UtcNow;

        var otpLog = new OtpLog
        {
            IdTaiKhoan = idTaiKhoan,
            MaOtp = otp,
            MucDich = MucDichOtp.DatLich,
            GioHetHan = now.AddMinutes(Math.Max(1, _options.ThoiHanPhut)),
            DaSuDung = false,
            SoLanThu = 0,
            NgayTao = now
        };

        _db.OtpLog.Add(otpLog);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "[OTP] Da tao OTP dat lich cho TaiKhoan #{IdTaiKhoan}, SDT {SoDienThoai}, het han sau {Phut} phut.",
            idTaiKhoan,
            soDienThoai,
            _options.ThoiHanPhut);

        return otp;
    }

    public async Task<bool> XacThucOtpDatLichAsync(int idTaiKhoan, string maOtp, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var otpLog = await _db.OtpLog
            .Where(x => x.IdTaiKhoan == idTaiKhoan
                        && x.MucDich == MucDichOtp.DatLich
                        && !x.DaSuDung
                        && x.GioHetHan > now)
            .OrderByDescending(x => x.NgayTao)
            .FirstOrDefaultAsync(cancellationToken);

        if (otpLog is null)
        {
            _logger.LogInformation("[OTP] Khong tim thay OTP dat lich con hieu luc cho TaiKhoan #{IdTaiKhoan}.", idTaiKhoan);
            return false;
        }

        otpLog.SoLanThu += 1;

        if (otpLog.SoLanThu > _options.SoLanNhapSaiToiDa)
        {
            otpLog.DaSuDung = true;
            await _db.SaveChangesAsync(cancellationToken);
            _logger.LogWarning("[OTP] TaiKhoan #{IdTaiKhoan} vuot qua so lan thu OTP cho phep.", idTaiKhoan);
            return false;
        }

        var ok = string.Equals(otpLog.MaOtp, maOtp, StringComparison.Ordinal);
        if (ok)
        {
            otpLog.DaSuDung = true;
            await _db.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("[OTP] Xac thuc OTP dat lich thanh cong cho TaiKhoan #{IdTaiKhoan}.", idTaiKhoan);
            return true;
        }

        await _db.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("[OTP] Xac thuc OTP dat lich that bai cho TaiKhoan #{IdTaiKhoan}.", idTaiKhoan);
        return false;
    }

    private static string TaoMaOtp()
    {
        Span<byte> bytes = stackalloc byte[4];
        RandomNumberGenerator.Fill(bytes);
        var value = BitConverter.ToUInt32(bytes);
        return (value % 1_000_000).ToString("D6");
    }
}
