using ClinicBooking.Domain.Enums;

namespace ClinicBooking.Application.Abstractions.Notifications;

/// <summary>
/// Contract giao tiep thong bao — Module 4 own va implement.
/// Semantic: tat ca method la fire-and-forget tu goc nhin cua handler —
/// khong throw exception lam hong flow chinh; loi gui thong bao duoc log va nuot.
/// </summary>
public interface INotificationService
{
    // ── Module 1: Dat lich hen & Hang cho ──────────────────────────────

    /// <summary>Gui thong bao "Lich hen da tao" cho benh nhan.</summary>
    Task GuiThongBaoTaoLichHenAsync(int idLichHen, CancellationToken cancellationToken = default);

    /// <summary>Gui thong bao "Lich hen da duoc xac nhan" cho benh nhan.</summary>
    Task GuiThongBaoXacNhanLichHenAsync(int idLichHen, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gui thong bao "Lich hen da huy" cho benh nhan.
    /// </summary>
    /// <param name="doPhongKhamHuy">True neu phong kham huy; false neu benh nhan tu huy.</param>
    Task GuiThongBaoHuyLichHenAsync(
        int idLichHen,
        string lyDo,
        bool doPhongKhamHuy,
        CancellationToken cancellationToken = default);

    /// <summary>Gui thong bao "Doi lich": thong tin lich cu + lich moi.</summary>
    Task GuiThongBaoDoiLichHenAsync(
        int idLichHenCu,
        int idLichHenMoi,
        CancellationToken cancellationToken = default);

    /// <summary>Gui thong bao "Check-in thanh cong" kem so thu tu hang cho.</summary>
    Task GuiThongBaoCheckInAsync(int idHangCho, CancellationToken cancellationToken = default);

    /// <summary>Gui thong bao "Moi benh nhan vao phong kham".</summary>
    Task GuiThongBaoGoiBenhNhanAsync(int idHangCho, CancellationToken cancellationToken = default);

    // ── Module 4: Thong bao tong quat ──────────────────────────────────

    /// <summary>
    /// Gui thong bao theo LoaiThongBao + kenh tuy chon.
    /// </summary>
    Task GuiAsync(
        int idTaiKhoanNhan,
        LoaiThongBao loai,
        Dictionary<string, string> duLieu,
        LoaiThamChieu? loaiThamChieu = null,
        int? idThamChieu = null,
        CancellationToken ct = default);
}