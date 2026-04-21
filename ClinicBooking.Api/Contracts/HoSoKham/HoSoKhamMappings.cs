using ClinicBooking.Application.Features.HoSoKham.Dtos;

namespace ClinicBooking.Api.Contracts.HoSoKham;

public static class HoSoKhamMappings
{
    public static HoSoKhamDto TuDto(this HoSoKhamResponse response) => new(
        response.IdHoSoKham,
        response.IdLichHen,
        response.MaLichHen,
        response.IdBenhNhan,
        response.HoTenBenhNhan,
        response.IdBacSi,
        response.HoTenBacSi,
        response.ChanDoan,
        response.KetQuaKham,
        response.GhiChu,
        response.NgayKham,
        response.NgayTao);

    public static HoSoKhamTomTatDto TuDto(this HoSoKhamTomTatResponse response) => new(
        response.IdHoSoKham,
        response.IdLichHen,
        response.MaLichHen,
        response.IdBacSi,
        response.HoTenBacSi,
        response.NgayKham,
        response.ChanDoan,
        response.KetQuaKham,
        response.NgayTao);
}
