using ClinicBooking.Domain.Enums;

namespace ClinicBooking.Application.Features.LichHen.Dtos;

/// <summary>
/// DTO chi tiet lich hen, dung cho GET /api/lich-hen/{id}.
/// </summary>
public record LichHenResponse(
    int IdLichHen,
    string MaLichHen,
    int IdBenhNhan,
    string HoTenBenhNhan,
    int IdCaLamViec,
    DateOnly NgayLamViec,
    TimeOnly GioBatDau,
    TimeOnly GioKetThuc,
    int IdDichVu,
    string TenDichVu,
    int SoSlot,
    HinhThucDat HinhThucDat,
    int? IdBacSiMongMuon,
    string? BacSiMongMuonNote,
    string? TrieuChung,
    TrangThaiLichHen TrangThai,
    DateTime NgayTao);
