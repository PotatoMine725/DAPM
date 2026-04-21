using ClinicBooking.Application.Features.DanhMuc.Dtos;

namespace ClinicBooking.Api.Contracts.DanhMuc;

public static class DinhNghiaCaMappings
{
    public static DinhNghiaCaDto TuDto(this DinhNghiaCaResponse response) => new(
        response.IdDinhNghiaCa,
        response.TenCa,
        response.GioBatDauMacDinh,
        response.GioKetThucMacDinh,
        response.MoTa,
        response.TrangThai);
}
