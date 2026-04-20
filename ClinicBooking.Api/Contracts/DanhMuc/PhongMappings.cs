using ClinicBooking.Application.Features.DanhMuc.Dtos;

namespace ClinicBooking.Api.Contracts.DanhMuc;

public static class PhongMappings
{
    public static PhongDto TuDto(this PhongResponse response) => new(
        response.IdPhong,
        response.MaPhong,
        response.TenPhong,
        response.SucChua,
        response.TrangBi,
        response.TrangThai);
}
