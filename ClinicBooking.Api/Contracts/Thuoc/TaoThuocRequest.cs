namespace ClinicBooking.Api.Contracts.Thuoc;

public record TaoThuocRequest(
    string TenThuoc,
    string? HoatChat,
    string? DonVi,
    string? GhiChu);
