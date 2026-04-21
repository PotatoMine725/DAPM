namespace ClinicBooking.Api.Contracts.HoSoKham;

public record CapNhatHoSoKhamRequest(
    string? ChanDoan,
    string? KetQuaKham,
    string? GhiChu);
