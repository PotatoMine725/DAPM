using ClinicBooking.Application.Features.Doctors.Dtos;

namespace ClinicBooking.Api.Contracts.Doctors;

public static class BacSiPublicMappings
{
    public static BacSiPublicDto TuDto(this BacSiPublicResponse response) => new(
        response.IdBacSi,
        response.IdChuyenKhoa,
        response.HoTen,
        response.AnhDaiDien,
        response.BangCap,
        response.NamKinhNghiem,
        response.TenChuyenKhoa);
}
