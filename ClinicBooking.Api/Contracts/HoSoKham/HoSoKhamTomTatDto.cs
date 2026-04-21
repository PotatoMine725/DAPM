namespace ClinicBooking.Api.Contracts.HoSoKham;

public record HoSoKhamTomTatDto(
    int IdHoSoKham,
    int IdLichHen,
    string MaLichHen,
    int IdBacSi,
    string HoTenBacSi,
    DateTime NgayKham,
    string? ChanDoan,
    string? KetQuaKham,
    DateTime NgayTao);
