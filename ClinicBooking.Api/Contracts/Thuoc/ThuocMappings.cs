using ClinicBooking.Application.Features.Thuoc.Dtos;

namespace ClinicBooking.Api.Contracts.Thuoc;

public static class ThuocMappings
{
    public static ThuocDto TuDto(this ThuocResponse response) => new(
        response.IdThuoc,
        response.TenThuoc,
        response.HoatChat,
        response.DonVi,
        response.GhiChu);
}
