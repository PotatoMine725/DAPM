using ClinicBooking.Application.Features.BacSi.Dtos;

namespace ClinicBooking.Api.Contracts.BacSi;

public static class BacSiMappings
{
    public static BacSiDto TuDto(this BacSiResponse response) => new(
        response.IdBacSi,
        response.IdTaiKhoan,
        response.IdChuyenKhoa,
        response.HoTen,
        response.AnhDaiDien,
        response.BangCap,
        response.NamKinhNghiem,
        response.TieuSu,
        response.LoaiHopDong,
        response.TrangThai,
        response.TenChuyenKhoa);
}
