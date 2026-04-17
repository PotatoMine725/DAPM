using ClinicBooking.Domain.Enums;

namespace ClinicBooking.Application.Features.LichHen.Dtos;

/// <summary>
/// DTO tom tat cho danh sach lich hen (danh sach cua toi, theo ngay).
/// </summary>
public record LichHenTomTatResponse(
    int IdLichHen,
    string MaLichHen,
    int IdBenhNhan,
    string HoTenBenhNhan,
    int IdCaLamViec,
    DateOnly NgayLamViec,
    TimeOnly GioBatDau,
    int SoSlot,
    string TenDichVu,
    TrangThaiLichHen TrangThai,
    DateTime NgayTao);
