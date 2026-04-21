namespace ClinicBooking.Application.Features.HoSoKham.Dtos;

public sealed record HoSoKhamTomTatResponse(
    int IdHoSoKham,
    int IdLichHen,
    string MaLichHen,
    int IdBacSi,
    string HoTenBacSi,
    DateTime NgayKham,
    string? ChanDoan,
    string? KetQuaKham,
    DateTime NgayTao);
