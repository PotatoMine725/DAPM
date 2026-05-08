using ClinicBooking.Application.Abstractions.Notifications;
using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Domain.Entities;
using ClinicBooking.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClinicBooking.Infrastructure.Services.Notifications;

/// <summary>
/// Ghi ThongBao vao DB (in-app). Khong gui email — do Module 4 dam nhan.
/// </summary>
public class NotificationServiceStub : INotificationService
{
    private readonly IAppDbContext _db;
    private readonly IDateTimeProvider _clock;
    private readonly ILogger<NotificationServiceStub> _logger;

    public NotificationServiceStub(
        IAppDbContext db,
        IDateTimeProvider clock,
        ILogger<NotificationServiceStub> logger)
    {
        _db = db;
        _clock = clock;
        _logger = logger;
    }

    public async Task GuiThongBaoTaoLichHenAsync(int idLichHen, CancellationToken ct = default)
    {
        var lh = await _db.LichHen
            .Include(x => x.BenhNhan)
            .FirstOrDefaultAsync(x => x.IdLichHen == idLichHen, ct);
        if (lh is null) return;

        await GhiThongBao(
            idTaiKhoan: lh.BenhNhan.IdTaiKhoan,
            idMau: 1,
            tieuDe: $"Đặt lịch hẹn thành công — {lh.MaLichHen}",
            noiDung: $"Lịch hẹn {lh.MaLichHen} đã được ghi nhận và đang chờ xác nhận từ phòng khám.",
            kenhGui: KenhGui.TrongApp,
            idThamChieu: idLichHen,
            loaiThamChieu: LoaiThamChieu.LichHen,
            ct);
    }

    public async Task GuiThongBaoXacNhanLichHenAsync(int idLichHen, CancellationToken ct = default)
    {
        var lh = await _db.LichHen
            .Include(x => x.BenhNhan)
            .FirstOrDefaultAsync(x => x.IdLichHen == idLichHen, ct);
        if (lh is null) return;

        await GhiThongBao(
            idTaiKhoan: lh.BenhNhan.IdTaiKhoan,
            idMau: 1,
            tieuDe: $"Lịch hẹn đã được xác nhận — {lh.MaLichHen}",
            noiDung: $"Lịch hẹn {lh.MaLichHen} đã được phòng khám xác nhận. Vui lòng đến trước giờ hẹn 15 phút.",
            kenhGui: KenhGui.TrongApp,
            idThamChieu: idLichHen,
            loaiThamChieu: LoaiThamChieu.LichHen,
            ct);
    }

    public async Task GuiThongBaoHuyLichHenAsync(
        int idLichHen, string lyDo, bool doPhongKhamHuy, CancellationToken ct = default)
    {
        var lh = await _db.LichHen
            .Include(x => x.BenhNhan)
            .FirstOrDefaultAsync(x => x.IdLichHen == idLichHen, ct);
        if (lh is null) return;

        var nguonHuy = doPhongKhamHuy ? "Phòng khám" : "Bạn";
        await GhiThongBao(
            idTaiKhoan: lh.BenhNhan.IdTaiKhoan,
            idMau: 4,
            tieuDe: $"Lịch hẹn đã bị hủy — {lh.MaLichHen}",
            noiDung: $"{nguonHuy} đã hủy lịch hẹn {lh.MaLichHen}. Lý do: {lyDo}.",
            kenhGui: KenhGui.TrongApp,
            idThamChieu: idLichHen,
            loaiThamChieu: LoaiThamChieu.LichHen,
            ct);
    }

    public async Task GuiThongBaoDoiLichHenAsync(
        int idLichHenCu, int idLichHenMoi, CancellationToken ct = default)
    {
        var lhMoi = await _db.LichHen
            .Include(x => x.BenhNhan)
            .FirstOrDefaultAsync(x => x.IdLichHen == idLichHenMoi, ct);
        if (lhMoi is null) return;

        await GhiThongBao(
            idTaiKhoan: lhMoi.BenhNhan.IdTaiKhoan,
            idMau: 1,
            tieuDe: $"Đổi lịch hẹn thành công — {lhMoi.MaLichHen}",
            noiDung: $"Lịch hẹn của bạn đã được đổi sang mã {lhMoi.MaLichHen} và đang chờ xác nhận.",
            kenhGui: KenhGui.TrongApp,
            idThamChieu: idLichHenMoi,
            loaiThamChieu: LoaiThamChieu.LichHen,
            ct);
    }

    public async Task GuiThongBaoCheckInAsync(int idHangCho, CancellationToken ct = default)
    {
        var hc = await _db.HangCho
            .Include(x => x.LichHen).ThenInclude(x => x.BenhNhan)
            .FirstOrDefaultAsync(x => x.IdHangCho == idHangCho, ct);
        if (hc is null) return;

        await GhiThongBao(
            idTaiKhoan: hc.LichHen.BenhNhan.IdTaiKhoan,
            idMau: 5,
            tieuDe: "Check-in thành công",
            noiDung: $"Bạn đã check-in thành công. Số thứ tự của bạn là {hc.SoThuTu}. Vui lòng chờ gọi tên.",
            kenhGui: KenhGui.TrongApp,
            idThamChieu: hc.IdLichHen,
            loaiThamChieu: LoaiThamChieu.LichHen,
            ct);
    }

    public async Task GuiThongBaoGoiBenhNhanAsync(int idHangCho, CancellationToken ct = default)
    {
        var hc = await _db.HangCho
            .Include(x => x.LichHen).ThenInclude(x => x.BenhNhan)
            .FirstOrDefaultAsync(x => x.IdHangCho == idHangCho, ct);
        if (hc is null) return;

        await GhiThongBao(
            idTaiKhoan: hc.LichHen.BenhNhan.IdTaiKhoan,
            idMau: 5,
            tieuDe: "Đến lượt khám của bạn",
            noiDung: $"Đến lượt khám của bạn (STT {hc.SoThuTu}). Vui lòng vào phòng khám ngay.",
            kenhGui: KenhGui.TrongApp,
            idThamChieu: hc.IdLichHen,
            loaiThamChieu: LoaiThamChieu.LichHen,
            ct);
    }

    private async Task GhiThongBao(
        int idTaiKhoan, int idMau,
        string tieuDe, string noiDung,
        KenhGui kenhGui,
        int? idThamChieu, LoaiThamChieu? loaiThamChieu,
        CancellationToken ct)
    {
        try
        {
            _db.ThongBao.Add(new ThongBao
            {
                IdTaiKhoan = idTaiKhoan,
                IdMau = idMau,
                TieuDe = tieuDe,
                NoiDung = noiDung,
                KenhGui = kenhGui,
                DaDoc = false,
                NgayGui = _clock.UtcNow,
                IdThamChieu = idThamChieu,
                LoaiThamChieu = loaiThamChieu,
            });
            await _db.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Notification] Loi ghi ThongBao cho TaiKhoan #{IdTaiKhoan}", idTaiKhoan);
        }
    }
}
