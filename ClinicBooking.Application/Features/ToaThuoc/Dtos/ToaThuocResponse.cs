namespace ClinicBooking.Application.Features.ToaThuoc.Dtos;

public sealed record ToaThuocResponse(
    int IdToaThuoc,
    int IdHoSoKham,
    int IdThuoc,
    string TenThuoc,
    string? LieuLuong,
    string? CachDung,
    int? SoNgayDung,
    string? GhiChu,
    DateTime NgayKham,
    string MaLichHen);
