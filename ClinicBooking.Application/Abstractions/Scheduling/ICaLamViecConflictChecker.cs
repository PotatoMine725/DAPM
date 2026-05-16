using ClinicBooking.Application.Common.Exceptions;

namespace ClinicBooking.Application.Abstractions.Scheduling;

public interface ICaLamViecConflictChecker
{
    /// <summary>
    /// Kiem tra ca moi co xung dot voi ca hien co cua bac si, phong, hoac don nghi phep.
    /// Throw <see cref="ConflictException"/> kem chi tiet xung dot neu co.
    /// </summary>
    Task EnsureKhongXungDotAsync(
        int idBacSi,
        int idPhong,
        DateOnly ngayLamViec,
        TimeOnly gioBatDau,
        TimeOnly gioKetThuc,
        int? idCaLamViecBoQua,
        CancellationToken cancellationToken = default);
}
