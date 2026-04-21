namespace ClinicBooking.Api.Contracts.Thuoc;

public record ThuocDto(
    int IdThuoc,
    string TenThuoc,
    string? HoatChat,
    string? DonVi,
    string? GhiChu);
