namespace ClinicBooking.Application.Features.ToaThuoc.Dtos;

public sealed record ToaThuocChiTietInput(
    int IdThuoc,
    string? LieuLuong,
    string? CachDung,
    int? SoNgayDung,
    string? GhiChu);
