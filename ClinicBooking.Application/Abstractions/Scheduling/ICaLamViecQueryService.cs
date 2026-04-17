using ClinicBooking.Application.Abstractions.Scheduling.Dtos;

namespace ClinicBooking.Application.Abstractions.Scheduling;

/// <summary>
/// Contract do Module 1 (Dat lich hen) tac gia, Module 2 (Lich lam viec) se implement that.
/// Tam thoi Module 1 ship mot stub trong Infrastructure de DI resolve duoc.
/// Tat ca cac thao tac doi voi <c>CaLamViec.SoSlotDaDat</c> PHAI di qua interface nay —
/// handler cua Module 1 khong duoc write truc tiep len cot do.
/// </summary>
public interface ICaLamViecQueryService
{
    /// <summary>Lay thong tin doc cua mot ca lam viec. Tra null neu khong ton tai.</summary>
    Task<ThongTinCaLamViecDto?> LayThongTinCaAsync(int idCaLamViec, CancellationToken cancellationToken = default);

    /// <summary>
    /// Kiem tra xem con co the them mot lich hen/giu cho moi cho ca hay khong.
    /// Tinh ca so giu cho tam thoi con hieu luc vao ket qua.
    /// </summary>
    Task<KetQuaKiemTraSlotDto> KiemTraSlotTrongAsync(int idCaLamViec, CancellationToken cancellationToken = default);

    /// <summary>True neu ca dang o trang thai <c>DaDuyet</c>.</summary>
    Task<bool> LaCaDuocDuyetAsync(int idCaLamViec, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cong don nguyen tu vao <c>CaLamViec.SoSlotDaDat</c> va tra ve gia tri moi.
    /// Delta duong khi dat lich, am khi huy. Noi bo dung atomic UPDATE (khong Read-Modify-Write tu client).
    /// </summary>
    /// <returns>Gia tri moi cua <c>SoSlotDaDat</c> sau khi cong.</returns>
    Task<int> IncrementSoSlotDaDatAsync(int idCaLamViec, int delta, CancellationToken cancellationToken = default);
}
