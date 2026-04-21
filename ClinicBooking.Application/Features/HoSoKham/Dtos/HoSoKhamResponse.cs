namespace ClinicBooking.Application.Features.HoSoKham.Dtos;

public sealed record HoSoKhamResponse(
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
    DateTime NgayTao)
{
    public static HoSoKhamResponse TuEntity(ClinicBooking.Domain.Entities.HoSoKham entity) => new(
        entity.IdHoSoKham,
        entity.IdLichHen,
        entity.LichHen.MaLichHen,
        entity.LichHen.IdBenhNhan,
        entity.LichHen.BenhNhan.HoTen,
        entity.IdBacSi,
        entity.BacSi.HoTen,
        entity.ChanDoan,
        entity.KetQuaKham,
        entity.GhiChu,
        entity.NgayKham,
        entity.NgayTao);
}
