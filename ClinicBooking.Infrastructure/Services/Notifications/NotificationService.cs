using ClinicBooking.Application.Abstractions.Notifications;
using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Domain.Entities;
using ClinicBooking.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClinicBooking.Infrastructure.Services.Notifications;

/// <summary>
/// Implementation thật cho <see cref="INotificationService"/>.
/// Ghi thông báo vào DB và gửi email.
/// Module 4 - Thông báo.
/// </summary>
public sealed class NotificationService : INotificationService
{
    private readonly IAppDbContext _db;
    private readonly IEmailService _emailService;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IAppDbContext db,
        IEmailService emailService,
        ILogger<NotificationService> logger)
    {
        _db = db;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task GuiThongBaoTaoLichHenAsync(int idLichHen, CancellationToken cancellationToken = default)
    {
        try
        {
            var lichHen = await _db.LichHen
                .Include(lh => lh.BenhNhan)
                .Include(lh => lh.CaLamViec)
                .ThenInclude(ca => ca.BacSi)
                .FirstOrDefaultAsync(lh => lh.IdLichHen == idLichHen, cancellationToken);

            if (lichHen == null)
            {
                _logger.LogWarning("[NotificationService] LichHen #{IdLichHen} khong ton tai", idLichHen);
                return;
            }

            // Ghi thông báo in-app
            var thongBao = new ThongBao
            {
                IdTaiKhoan = lichHen.IdBenhNhan,
                IdMau = 1, // TODO: Get appropriate template ID
                KenhGui = KenhGui.TrongApp,
                TieuDe = "Xác nhận lịch hẹn khám",
                NoiDung = $"Bạn đã đặt lịch khám thành công với BS {lichHen.CaLamViec.BacSi.HoTen} vào lúc {lichHen.CaLamViec.GioBatDau:HH:mm} ngày {lichHen.CaLamViec.NgayLamViec:dd/MM/yyyy}. Vui lòng đến trước 15 phút.",
                DaDoc = false,
                NgayGui = DateTime.UtcNow,
                LoaiThamChieu = LoaiThamChieu.LichHen,
                IdThamChieu = lichHen.IdLichHen
            };

            _db.ThongBao.Add(thongBao);
            await _db.SaveChangesAsync(cancellationToken);

            // Gửi email
            var emailTo = lichHen.BenhNhan.TaiKhoan.Email;
            if (!string.IsNullOrEmpty(emailTo))
            {
                var subject = "Xác nhận lịch hẹn khám - Phòng khám";
                var body = $@"
Kính gửi {lichHen.BenhNhan.HoTen},

Bạn đã đặt lịch khám thành công với thông tin chi tiết:
- Bác sĩ: {lichHen.CaLamViec.BacSi.HoTen}
- Thời gian: {lichHen.CaLamViec.GioBatDau:HH:mm} - {lichHen.CaLamViec.GioKetThuc:HH:mm}
- Ngày khám: {lichHen.CaLamViec.NgayLamViec:dd/MM/yyyy}
- Chuyên khoa: {lichHen.CaLamViec.BacSi.ChuyenKhoa.TenChuyenKhoa}

Vui lòng đến trước 15 phút để làm thủ tục check-in.

Trân trọng,
Phòng khám";

                await _emailService.GuiEmailAsync(emailTo, subject, body, false, cancellationToken);
            }

            _logger.LogInformation("[NotificationService] Da gui thong bao tao LichHen #{IdLichHen}", idLichHen);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[NotificationService] Loi khi gui thong bao tao LichHen #{IdLichHen}", idLichHen);
        }
    }

    public async Task GuiThongBaoXacNhanLichHenAsync(int idLichHen, CancellationToken cancellationToken = default)
    {
        try
        {
            var lichHen = await _db.LichHen
                .Include(lh => lh.BenhNhan)
                .Include(lh => lh.CaLamViec)
                .ThenInclude(ca => ca.BacSi)
                .FirstOrDefaultAsync(lh => lh.IdLichHen == idLichHen, cancellationToken);

            if (lichHen == null)
            {
                _logger.LogWarning("[NotificationService] LichHen #{IdLichHen} khong ton tai", idLichHen);
                return;
            }

            // Ghi thông báo in-app
            var thongBao = new ThongBao
            {
                IdTaiKhoan = lichHen.IdBenhNhan,
                IdMau = 2, // TODO: Get appropriate template ID
                KenhGui = KenhGui.TrongApp,
                TieuDe = "Lịch hẹn đã được xác nhận",
                NoiDung = $"Lịch hẹn của bạn với BS {lichHen.CaLamViec.BacSi.HoTen} vào lúc {lichHen.CaLamViec.GioBatDau:HH:mm} ngày {lichHen.CaLamViec.NgayLamViec:dd/MM/yyyy} đã được lễ tân xác nhận.",
                DaDoc = false,
                NgayGui = DateTime.UtcNow,
                LoaiThamChieu = LoaiThamChieu.LichHen,
                IdThamChieu = lichHen.IdLichHen
            };

            _db.ThongBao.Add(thongBao);
            await _db.SaveChangesAsync(cancellationToken);

            // Gửi email
            var emailTo = lichHen.BenhNhan.TaiKhoan.Email;
            if (!string.IsNullOrEmpty(emailTo))
            {
                var subject = "Lịch hẹn đã được xác nhận - Phòng khám";
                var body = $@"
Kính gửi {lichHen.BenhNhan.HoTen},

Lịch hẹn của bạn đã được lễ tân xác nhận:
- Bác sĩ: {lichHen.CaLamViec.BacSi.HoTen}
- Thời gian: {lichHen.CaLamViec.GioBatDau:HH:mm} - {lichHen.CaLamViec.GioKetThuc:HH:mm}
- Ngày khám: {lichHen.CaLamViec.NgayLamViec:dd/MM/yyyy}

Vui lòng đến trước 15 phút để làm thủ tục check-in.

Trân trọng,
Phòng khám";

                await _emailService.GuiEmailAsync(emailTo, subject, body, false, cancellationToken);
            }

            _logger.LogInformation("[NotificationService] Da gui thong bao xac nhan LichHen #{IdLichHen}", idLichHen);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[NotificationService] Loi khi gui thong bao xac nhan LichHen #{IdLichHen}", idLichHen);
        }
    }

    public async Task GuiThongBaoHuyLichHenAsync(
        int idLichHen, string lyDo, bool doPhongKhamHuy, CancellationToken cancellationToken = default)
    {
        try
        {
            var lichHen = await _db.LichHen
                .Include(lh => lh.BenhNhan)
                .Include(lh => lh.CaLamViec)
                .ThenInclude(ca => ca.BacSi)
                .FirstOrDefaultAsync(lh => lh.IdLichHen == idLichHen, cancellationToken);

            if (lichHen == null)
            {
                _logger.LogWarning("[NotificationService] LichHen #{IdLichHen} khong ton tai", idLichHen);
                return;
            }

            // Ghi thông báo in-app
            var thongBao = new ThongBao
            {
                IdTaiKhoan = lichHen.IdBenhNhan,
                IdMau = 3, // TODO: Get appropriate template ID
                KenhGui = KenhGui.TrongApp,
                TieuDe = "Lịch hẹn đã bị hủy",
                NoiDung = $"Lịch hẹn của bạn với BS {lichHen.CaLamViec.BacSi.HoTen} vào lúc {lichHen.CaLamViec.GioBatDau:HH:mm} ngày {lichHen.CaLamViec.NgayLamViec:dd/MM/yyyy} đã bị hủy. Lý do: {lyDo}",
                DaDoc = false,
                NgayGui = DateTime.UtcNow,
                LoaiThamChieu = LoaiThamChieu.LichHen,
                IdThamChieu = lichHen.IdLichHen
            };

            _db.ThongBao.Add(thongBao);
            await _db.SaveChangesAsync(cancellationToken);

            // Gửi email
            var emailTo = lichHen.BenhNhan.TaiKhoan.Email;
            if (!string.IsNullOrEmpty(emailTo))
            {
                var subject = "Lịch hẹn đã bị hủy - Phòng khám";
                var body = $@"
Kính gửi {lichHen.BenhNhan.HoTen},

Lịch hẹn của bạn đã bị hủy với thông tin:
- Bác sĩ: {lichHen.CaLamViec.BacSi.HoTen}
- Thời gian: {lichHen.CaLamViec.GioBatDau:HH:mm} - {lichHen.CaLamViec.GioKetThuc:HH:mm}
- Ngày khám: {lichHen.CaLamViec.NgayLamViec:dd/MM/yyyy}
- Lý do hủy: {lyDo}
- {(doPhongKhamHuy ? "Hủy bởi phòng khám" : "Hủy bởi bạn")}

Nếu bạn cần đặt lại lịch, vui lòng truy cập hệ thống.

Trân trọng,
Phòng khám";

                await _emailService.GuiEmailAsync(emailTo, subject, body, false, cancellationToken);
            }

            _logger.LogInformation("[NotificationService] Da gui thong bao huy LichHen #{IdLichHen}", idLichHen);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[NotificationService] Loi khi gui thong bao huy LichHen #{IdLichHen}", idLichHen);
        }
    }

    public async Task GuiThongBaoDoiLichHenAsync(
        int idLichHenCu, int idLichHenMoi, CancellationToken cancellationToken = default)
    {
        try
        {
            var lichHenCu = await _db.LichHen
                .Include(lh => lh.BenhNhan)
                .Include(lh => lh.CaLamViec)
                .ThenInclude(ca => ca.BacSi)
                .FirstOrDefaultAsync(lh => lh.IdLichHen == idLichHenCu, cancellationToken);

            var lichHenMoi = await _db.LichHen
                .Include(lh => lh.CaLamViec)
                .ThenInclude(ca => ca.BacSi)
                .FirstOrDefaultAsync(lh => lh.IdLichHen == idLichHenMoi, cancellationToken);

            if (lichHenCu == null || lichHenMoi == null)
            {
                _logger.LogWarning("[NotificationService] LichHen cu #{IdLichHenCu} hoặc moi #{IdLichHenMoi} khong ton tai", idLichHenCu, idLichHenMoi);
                return;
            }

            // Ghi thông báo in-app
            var thongBao = new ThongBao
            {
                IdTaiKhoan = lichHenCu.IdBenhNhan,
                IdMau = 4, // TODO: Get appropriate template ID
                KenhGui = KenhGui.TrongApp,
                TieuDe = "Lịch hẹn đã được đổi",
                NoiDung = $"Lịch hẹn của bạn đã được đổi từ BS {lichHenCu.CaLamViec.BacSi.HoTen} ({lichHenCu.CaLamViec.GioBatDau:HH:mm} {lichHenCu.CaLamViec.NgayLamViec:dd/MM/yyyy}) sang BS {lichHenMoi.CaLamViec.BacSi.HoTen} ({lichHenMoi.CaLamViec.GioBatDau:HH:mm} {lichHenMoi.CaLamViec.NgayLamViec:dd/MM/yyyy}).",
                DaDoc = false,
                NgayGui = DateTime.UtcNow,
                LoaiThamChieu = LoaiThamChieu.LichHen,
                IdThamChieu = lichHenMoi.IdLichHen
            };

            _db.ThongBao.Add(thongBao);
            await _db.SaveChangesAsync(cancellationToken);

            // Gửi email
            var emailTo = lichHenCu.BenhNhan.TaiKhoan.Email;
            if (!string.IsNullOrEmpty(emailTo))
            {
                var subject = "Lịch hẹn đã được đổi - Phòng khám";
                var body = $@"
Kính gửi {lichHenCu.BenhNhan.HoTen},

Lịch hẹn của bạn đã được đổi thành công:

Lịch cũ:
- Bác sĩ: {lichHenCu.CaLamViec.BacSi.HoTen}
- Thời gian: {lichHenCu.CaLamViec.GioBatDau:HH:mm} - {lichHenCu.CaLamViec.GioKetThuc:HH:mm}
- Ngày khám: {lichHenCu.CaLamViec.NgayLamViec:dd/MM/yyyy}

Lịch mới:
- Bác sĩ: {lichHenMoi.CaLamViec.BacSi.HoTen}
- Thời gian: {lichHenMoi.CaLamViec.GioBatDau:HH:mm} - {lichHenMoi.CaLamViec.GioKetThuc:HH:mm}
- Ngày khám: {lichHenMoi.CaLamViec.NgayLamViec:dd/MM/yyyy}

Vui lòng đến trước 15 phút để làm thủ tục check-in.

Trân trọng,
Phòng khám";

                await _emailService.GuiEmailAsync(emailTo, subject, body, false, cancellationToken);
            }

            _logger.LogInformation("[NotificationService] Da gui thong bao doi LichHen #{IdLichHenCu} -> #{IdLichHenMoi}", idLichHenCu, idLichHenMoi);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[NotificationService] Loi khi gui thong bao doi LichHen #{IdLichHenCu} -> #{IdLichHenMoi}", idLichHenCu, idLichHenMoi);
        }
    }

    public async Task GuiThongBaoCheckInAsync(int idHangCho, CancellationToken cancellationToken = default)
    {
        try
        {
            var hangCho = await _db.HangCho
                .Include(hc => hc.LichHen)
                .ThenInclude(lh => lh.BenhNhan)
                .Include(hc => hc.LichHen)
                .ThenInclude(lh => lh.CaLamViec)
                .ThenInclude(ca => ca.BacSi)
                .FirstOrDefaultAsync(hc => hc.IdHangCho == idHangCho, cancellationToken);

            if (hangCho == null)
            {
                _logger.LogWarning("[NotificationService] HangCho #{IdHangCho} khong ton tai", idHangCho);
                return;
            }

            // Ghi thông báo in-app
            var thongBao = new ThongBao
            {
                IdTaiKhoan = hangCho.LichHen.IdBenhNhan,
                IdMau = 5, // TODO: Get appropriate template ID
                KenhGui = KenhGui.TrongApp,
                TieuDe = "Check-in thành công",
                NoiDung = $"Bạn đã check-in thành công. Số thứ tự trong hàng chờ: {hangCho.SoThuTu}. Bác sĩ {hangCho.LichHen.CaLamViec.BacSi.HoTen} sẽ gọi khi đến lượt.",
                DaDoc = false,
                NgayGui = DateTime.UtcNow,
                LoaiThamChieu = LoaiThamChieu.LichHen,
                IdThamChieu = hangCho.IdHangCho
            };

            _db.ThongBao.Add(thongBao);
            await _db.SaveChangesAsync(cancellationToken);

            // Gửi email
            var emailTo = hangCho.LichHen.BenhNhan.TaiKhoan.Email;
            if (!string.IsNullOrEmpty(emailTo))
            {
                var subject = "Check-in thành công - Phòng khám";
                var body = $@"
Kính gửi {hangCho.LichHen.BenhNhan.HoTen},

Bạn đã check-in thành công với thông tin:
- Bác sĩ: {hangCho.LichHen.CaLamViec.BacSi.HoTen}
- Số thứ tự hàng chờ: {hangCho.SoThuTu}
- Phòng khám: {hangCho.LichHen.CaLamViec.Phong?.TenPhong ?? "Chưa phân phòng"}

Vui lòng chờ trong khu vực chờ, bác sĩ sẽ gọi khi đến lượt.

Trân trọng,
Phòng khám";

                await _emailService.GuiEmailAsync(emailTo, subject, body, false, cancellationToken);
            }

            _logger.LogInformation("[NotificationService] Da gui thong bao check-in HangCho #{IdHangCho}", idHangCho);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[NotificationService] Loi khi gui thong bao check-in HangCho #{IdHangCho}", idHangCho);
        }
    }

    public async Task GuiAsync(
        int idTaiKhoanNhan,
        LoaiThongBao loai,
        Dictionary<string, string> duLieu,
        LoaiThamChieu? loaiThamChieu = null,
        int? idThamChieu = null,
        CancellationToken ct = default)
    {
        try
        {
            var idMau = loai switch
            {
                LoaiThongBao.HuyLich => 3,
                LoaiThongBao.CheckIn => 5,
                _ => 1
            };
            duLieu.TryGetValue("tieuDe", out var tieuDe);
            duLieu.TryGetValue("noiDung", out var noiDung);
            _db.ThongBao.Add(new ThongBao
            {
                IdTaiKhoan = idTaiKhoanNhan,
                IdMau = idMau,
                KenhGui = KenhGui.TrongApp,
                TieuDe = tieuDe ?? loai.ToString(),
                NoiDung = noiDung ?? string.Join("; ", duLieu.Select(kv => $"{kv.Key}={kv.Value}")),
                DaDoc = false,
                NgayGui = DateTime.UtcNow,
                LoaiThamChieu = loaiThamChieu,
                IdThamChieu = idThamChieu
            });
            await _db.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[NotificationService] Loi GuiAsync loai {Loai} cho TaiKhoan #{IdTaiKhoan}", loai, idTaiKhoanNhan);
        }
    }

    public async Task GuiThongBaoGoiBenhNhanAsync(int idHangCho, CancellationToken cancellationToken = default)
    {
        try
        {
            var hangCho = await _db.HangCho
                .Include(hc => hc.LichHen)
                .ThenInclude(lh => lh.BenhNhan)
                .Include(hc => hc.LichHen)
                .ThenInclude(lh => lh.CaLamViec)
                .ThenInclude(ca => ca.BacSi)
                .FirstOrDefaultAsync(hc => hc.IdHangCho == idHangCho, cancellationToken);

            if (hangCho == null)
            {
                _logger.LogWarning("[NotificationService] HangCho #{IdHangCho} khong ton tai", idHangCho);
                return;
            }

            // Ghi thông báo in-app
            var thongBao = new ThongBao
            {
                IdTaiKhoan = hangCho.LichHen.IdBenhNhan,
                IdMau = 6, // TODO: Get appropriate template ID
                KenhGui = KenhGui.TrongApp,
                TieuDe = "Mời vào phòng khám",
                NoiDung = $"Bác sĩ {hangCho.LichHen.CaLamViec.BacSi.HoTen} mời bạn vào phòng khám. Số thứ tự: {hangCho.SoThuTu}.",
                DaDoc = false,
                NgayGui = DateTime.UtcNow,
                LoaiThamChieu = LoaiThamChieu.LichHen,
                IdThamChieu = hangCho.IdHangCho
            };

            _db.ThongBao.Add(thongBao);
            await _db.SaveChangesAsync(cancellationToken);

            // Gửi email
            var emailTo = hangCho.LichHen.BenhNhan.TaiKhoan.Email;
            if (!string.IsNullOrEmpty(emailTo))
            {
                var subject = "Mời vào phòng khám - Phòng khám";
                var body = $@"
Kính gửi {hangCho.LichHen.BenhNhan.HoTen},

Bác sĩ {hangCho.LichHen.CaLamViec.BacSi.HoTen} mời bạn vào phòng khám.
- Số thứ tự: {hangCho.SoThuTu}
- Phòng khám: {hangCho.LichHen.CaLamViec.Phong?.TenPhong ?? "Chưa phân phòng"}

Vui lòng đến ngay phòng khám để bắt đầu khám.

Trân trọng,
Phòng khám";

                await _emailService.GuiEmailAsync(emailTo, subject, body, false, cancellationToken);
            }

            _logger.LogInformation("[NotificationService] Da gui thong bao goi benh nhan HangCho #{IdHangCho}", idHangCho);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[NotificationService] Loi khi gui thong bao goi benh nhan HangCho #{IdHangCho}", idHangCho);
        }
    }
}
