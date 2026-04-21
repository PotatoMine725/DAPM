namespace ClinicBooking.Api.Contracts.HoSoKham;

public record TaoHoSoKhamRequest(
    int IdLichHen,
    string? ChanDoan,
    string? KetQuaKham,
    string? GhiChu);
