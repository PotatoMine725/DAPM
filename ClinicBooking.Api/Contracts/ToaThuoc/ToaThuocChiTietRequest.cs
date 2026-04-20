namespace ClinicBooking.Api.Contracts.ToaThuoc;

public record ToaThuocChiTietRequest(
    int IdThuoc,
    string? LieuLuong,
    string? CachDung,
    int? SoNgayDung,
    string? GhiChu);
