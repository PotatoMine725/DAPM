using System.Security.Cryptography;
using ClinicBooking.Application.Abstractions.Notifications;
using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Options;
using ClinicBooking.Domain.Entities;
using ClinicBooking.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ClinicBooking.Infrastructure.Services.Security;

public sealed class OtpService : IOtpService
{
    private readonly IAppDbContext _db;
    private readonly IEmailService _emailService;
    private readonly ILogger<OtpService> _logger;
    private readonly OtpOptions _options;
    private readonly IHostEnvironment _env;

    public OtpService(
        IAppDbContext db,
        IEmailService emailService,
        ILogger<OtpService> logger,
        IOptions<OtpOptions> options,
        IHostEnvironment env)
    {
        _db = db;
        _emailService = emailService;
        _logger = logger;
        _options = options.Value;
        _env = env;
    }

    public async Task<string> TaoVaGuiOtpDatLichAsync(
        int idTaiKhoan,
        string soDienThoai,
        string? emailGui = null,
        CancellationToken cancellationToken = default)
    {
        var otp = TaoMaOtp();
        var now = DateTime.UtcNow;

        // Invalidate toàn bộ OTP active cũ của account này
        var otpCu = await _db.OtpLog
            .Where(ol => ol.IdTaiKhoan == idTaiKhoan && !ol.DaSuDung)
            .ToListAsync(cancellationToken);

        foreach (var o in otpCu)
            o.DaSuDung = true;

        _db.OtpLog.Add(new OtpLog
        {
            IdTaiKhoan = idTaiKhoan,
            MaOtp      = otp,
            MucDich    = MucDichOtp.DatLich,
            GioHetHan  = now.AddMinutes(Math.Max(1, _options.ThoiHanPhut)),
            DaSuDung   = false,
            SoLanThu   = 0,
            NgayTao    = now
        });

        await _db.SaveChangesAsync(cancellationToken);

        // Ưu tiên emailGui được truyền vào; fallback đọc từ DB
        var emailDich = !string.IsNullOrWhiteSpace(emailGui)
            ? emailGui
            : await LayEmailDichAsync(idTaiKhoan, cancellationToken);

        await GuiOtpQuaEmailAsync(idTaiKhoan, otp, emailDich, cancellationToken);

        return otp;
    }

    public async Task<bool> XacThucOtpDatLichAsync(
        int idTaiKhoan,
        string maOtp,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var otpLog = await _db.OtpLog
            .Where(ol =>
                ol.IdTaiKhoan == idTaiKhoan &&
                ol.MucDich    == MucDichOtp.DatLich &&
                !ol.DaSuDung  &&
                ol.GioHetHan  > now)
            .OrderByDescending(ol => ol.NgayTao)
            .FirstOrDefaultAsync(cancellationToken);

        if (otpLog is null)
        {
            _logger.LogInformation("[OTP] Khong tim thay OTP DatLich con hieu luc cho TaiKhoan #{Id}.", idTaiKhoan);
            return false;
        }

        otpLog.SoLanThu += 1;

        if (otpLog.SoLanThu > _options.SoLanNhapSaiToiDa)
        {
            otpLog.DaSuDung = true;
            await _db.SaveChangesAsync(cancellationToken);
            _logger.LogWarning("[OTP] TaiKhoan #{Id} vuot qua so lan thu OTP.", idTaiKhoan);
            return false;
        }

        var ok = string.Equals(otpLog.MaOtp, maOtp, StringComparison.Ordinal);
        if (ok)
            otpLog.DaSuDung = true;

        await _db.SaveChangesAsync(cancellationToken);

        if (ok)
            _logger.LogInformation("[OTP] Xac thuc OTP DatLich thanh cong cho TaiKhoan #{Id}.", idTaiKhoan);
        else
            _logger.LogInformation("[OTP] Ma OTP sai cho TaiKhoan #{Id} (lan {Lan}/{Max}).", idTaiKhoan, otpLog.SoLanThu, _options.SoLanNhapSaiToiDa);

        return ok;
    }

    // --- helpers ---

    private async Task<string?> LayEmailDichAsync(int idTaiKhoan, CancellationToken ct)
    {
        var taiKhoan = await _db.TaiKhoan
            .AsNoTracking()
            .FirstOrDefaultAsync(tk => tk.IdTaiKhoan == idTaiKhoan, ct);

        if (taiKhoan is null) return null;

        var email = taiKhoan.Email;

        // Ghost account email không dùng được
        if (string.IsNullOrWhiteSpace(email) || email.EndsWith("@local.invalid"))
            return null;

        return email;
    }

    private async Task GuiOtpQuaEmailAsync(
        int idTaiKhoan,
        string otp,
        string? emailDich,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(emailDich))
        {
            if (_env.IsDevelopment())
                _logger.LogWarning(
                    "[OTP-DEV] TaiKhoan #{Id} khong co email hop le. Ma OTP (dev only): {Otp}",
                    idTaiKhoan, otp);
            else
                _logger.LogWarning(
                    "[OTP] TaiKhoan #{Id} khong co email hop le, khong the gui OTP.", idTaiKhoan);
            return;
        }

        try
        {
            var subject = "Ma OTP xac nhan - Phong kham";
            var body = $@"<p>Ma OTP xac nhan cua ban la: <strong style=""font-size:1.4em;letter-spacing:.1em"">{otp}</strong></p>
<p>Ma co hieu luc trong <strong>{_options.ThoiHanPhut} phut</strong>. Khong chia se ma nay cho bat ky ai.</p>
<p><small>Neu ban khong thuc hien yeu cau nay, hay bo qua email nay.</small></p>";

            await _emailService.GuiEmailAsync(emailDich, subject, body, isHtml: true, ct);

            _logger.LogInformation("[OTP] Da gui OTP den {Email} cho TaiKhoan #{Id}.", emailDich, idTaiKhoan);
        }
        catch (Exception ex)
        {
            if (_env.IsDevelopment())
                _logger.LogWarning(ex,
                    "[OTP-DEV] Gui email that bai cho TaiKhoan #{Id}. Ma OTP (dev only): {Otp}",
                    idTaiKhoan, otp);
            else
                _logger.LogError(ex,
                    "[OTP] Gui email that bai cho TaiKhoan #{Id}.", idTaiKhoan);
        }
    }

    private static string TaoMaOtp()
    {
        Span<byte> bytes = stackalloc byte[4];
        RandomNumberGenerator.Fill(bytes);
        var value = BitConverter.ToUInt32(bytes);
        return (value % 1_000_000).ToString("D6");
    }
}
