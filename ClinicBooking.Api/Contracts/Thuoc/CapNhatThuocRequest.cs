namespace ClinicBooking.Api.Contracts.Thuoc;

public record CapNhatThuocRequest(
    string TenThuoc,
    string? HoatChat,
    string? DonVi,
    string? GhiChu);
