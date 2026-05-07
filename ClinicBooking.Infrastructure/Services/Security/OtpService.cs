using ClinicBooking.Application.Abstractions.Notifications;
using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Options;
using ClinicBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ClinicBooking.Infrastructure.Services.Security;

/// <summary>
/// Implementation thật cho <see cref="IOtpService"/>.
/// Gửi OTP qua email thật thay vì hardcode "123456".
/// Module 4 - Thông báo.
/// </summary>
public sealed class OtpService : IOtpService
{
    private readonly IAppDbContext _db;
    private readonly IEmailService _emailService;
    private readonly ILogger<OtpService> _logger;
    private readonly OtpOptions _options;

    public OtpService(
        IAppDbContext db,
        IEmailService emailService,
        ILogger<OtpService> logger,
        IOptions<OtpOptions> options)
    {
        _db = db;
        _emailService = emailService;
        _logger = logger;
        _options = options.Value;
    }

    public async Task<string> TaoVaGuiOtpDatLichAsync(int idTaiKhoan, string soDienThoai, CancellationToken cancellationToken = default)
    {
        try
        {
            // Tạo OTP ngẫu nhiên 6 số
            var random = new Random();
            var otp = random.Next(100000, 999999).ToString();
            
            var now = DateTime.UtcNow;
            var hetHan = now.AddMinutes(_options.ThoiHanPhut);

            // Lấy thông tin tài khoản để lấy email
            var taiKhoan = await _db.TaiKhoan
                .FirstOrDefaultAsync(tk => tk.IdTaiKhoan == idTaiKhoan, cancellationToken);

            if (taiKhoan == null)
            {
                _logger.LogWarning("[OtpService] TaiKhoan #{IdTaiKhoan} khong ton tai", idTaiKhoan);
                throw new Exception("Tài khoản không tồn tại");
            }

            // Lấy thông tin bệnh nhân nếu có
            BenhNhan? benhNhan = null;
            if (taiKhoan.VaiTro == Domain.Enums.VaiTro.BenhNhan)
            {
                benhNhan = await _db.BenhNhan
                    .FirstOrDefaultAsync(bn => bn.IdTaiKhoan == idTaiKhoan, cancellationToken);
            }

            // Ghi OTP log vào DB
            var otpLog = new OtpLog
            {
                IdTaiKhoan = idTaiKhoan,
                
                MaOtp = otp,
                GioHetHan = hetHan,
                DaSuDung = false,
                NgayTao = now
            };

            _db.OtpLog.Add(otpLog);
            
            // Cleanup các OTP cũ của cùng tài khoản
            var otpCu = await _db.OtpLog
                .Where(ol => ol.IdTaiKhoan == idTaiKhoan && !ol.DaSuDung && ol.GioHetHan < now)
                .ToListAsync(cancellationToken);
            
            if (otpCu.Count > 0)
            {
                _db.OtpLog.RemoveRange(otpCu);
            }

            await _db.SaveChangesAsync(cancellationToken);

            // Gửi email OTP
            var emailTo = taiKhoan.Email;
            if (string.IsNullOrEmpty(emailTo) && benhNhan != null)
            {
                emailTo = benhNhan.TaiKhoan.Email;
            }

            if (!string.IsNullOrEmpty(emailTo))
            {
                var subject = "Mã OTP xác nhận đặt lịch - Phòng khám";
                var body = $@"
Kính gửi {(benhNhan?.HoTen ?? "Bạn")},

Bạn đang thực hiện đặt lịch khám tại phòng khám.

Mã OTP xác nhận của bạn là: <strong>{otp}</strong>

Mã OTP có hiệu lực trong {_options.ThoiHanPhut} phút.

Nếu bạn không thực hiện thao tác này, vui lòng bỏ qua email này.

Trân trọng,
Phòng khám";

                await _emailService.GuiEmailAsync(emailTo, subject, body, true, cancellationToken);
                
                _logger.LogInformation(
                    "[OtpService] Da gui OTP {Otp} den email {Email} cho TaiKhoan #{IdTaiKhoan}",
                    otp, emailTo, idTaiKhoan);
            }
            else
            {
                _logger.LogWarning(
                    "[OtpService] TaiKhoan #{IdTaiKhoan} khong co email de gui OTP",
                    idTaiKhoan);
            }

            return otp;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[OtpService] Loi khi tao va gui OTP cho TaiKhoan #{IdTaiKhoan}", idTaiKhoan);
            throw;
        }
    }

    public async Task<bool> XacThucOtpDatLichAsync(int idTaiKhoan, string maOtp, CancellationToken cancellationToken = default)
    {
        try
        {
            var now = DateTime.UtcNow;

            var otpLog = await _db.OtpLog
                .FirstOrDefaultAsync(ol => 
                    ol.IdTaiKhoan == idTaiKhoan && 
                    ol.MaOtp == maOtp && 
                    !ol.DaSuDung && 
                    ol.GioHetHan >= now, 
                    cancellationToken);

            if (otpLog == null)
            {
                _logger.LogWarning(
                    "[OtpService] OTP {MaOtp} khong hop le cho TaiKhoan #{IdTaiKhoan}",
                    maOtp, idTaiKhoan);
                return false;
            }

            // Đánh dấu đã sử dụng
            otpLog.DaSuDung = true;
            // otpLog.NgaySuDung = now; // Property không tồn tại trong OtpLog
            
            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "[OtpService] Xac thuc OTP thanh cong cho TaiKhoan #{IdTaiKhoan}",
                idTaiKhoan);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[OtpService] Loi khi xac thuc OTP cho TaiKhoan #{IdTaiKhoan}", idTaiKhoan);
            return false;
        }
    }
}
