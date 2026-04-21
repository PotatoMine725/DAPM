namespace ClinicBooking.Api.Contracts.HoSoKham;

public record HoSoKhamDto(
    int IdHoSoKham,
    int IdLichHen,
    string MaLichHen,
    int IdBenhNhan,
    string HoTenBenhNhan,
    int IdBacSi,
    string HoTenBacSi,
    string? ChanDoan,
    string? KetQuaKham,
    string? GhiChu,
    DateTime NgayKham,
    DateTime NgayTao);
