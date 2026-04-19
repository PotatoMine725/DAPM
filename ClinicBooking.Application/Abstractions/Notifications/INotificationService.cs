namespace ClinicBooking.Application.Abstractions.Notifications;

/// <summary>
/// Contract do Module 1 tac gia, Module 4 (Thong bao) se implement that.
/// Tam thoi Module 1 ship <c>NotificationServiceStub</c> (chi log) de DI resolve duoc.
/// Semantic: tat ca method la fire-and-forget tu goc nhin cua handler —
/// khong throw exception lam hong flow chinh; loi gui thong bao duoc log va nuot.
/// </summary>
public interface INotificationService
{
    /// <summary>Gui thong bao "Lich hen da tao" cho benh nhan.</summary>
    Task GuiThongBaoTaoLichHenAsync(int idLichHen, CancellationToken cancellationToken = default);

    /// <summary>Gui thong bao "Lich hen da duoc xac nhan" cho benh nhan.</summary>
    Task GuiThongBaoXacNhanLichHenAsync(int idLichHen, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gui thong bao "Lich hen da huy" cho benh nhan.
    /// </summary>
    /// <param name="doPhongKhamHuy">True neu phong kham huy (ly do phia co so y te); false neu benh nhan tu huy.</param>
    Task GuiThongBaoHuyLichHenAsync(int idLichHen, string lyDo, bool doPhongKhamHuy, CancellationToken cancellationToken = default);

    /// <summary>Gui thong bao "Doi lich": thong tin lich cu + lich moi.</summary>
    Task GuiThongBaoDoiLichHenAsync(int idLichHenCu, int idLichHenMoi, CancellationToken cancellationToken = default);

    /// <summary>Gui thong bao "Check-in thanh cong" kem so thu tu hang cho.</summary>
    Task GuiThongBaoCheckInAsync(int idHangCho, CancellationToken cancellationToken = default);

    /// <summary>Gui thong bao "Moi benh nhan vao phong kham".</summary>
    Task GuiThongBaoGoiBenhNhanAsync(int idHangCho, CancellationToken cancellationToken = default);
}
