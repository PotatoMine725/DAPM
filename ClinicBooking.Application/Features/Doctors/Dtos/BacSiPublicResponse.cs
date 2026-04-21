using ClinicBooking.Domain.Entities;

namespace ClinicBooking.Application.Features.Doctors.Dtos;

public sealed record BacSiPublicResponse(
    int IdBacSi,
    int IdChuyenKhoa,
    string HoTen,
    string? AnhDaiDien,
    string? BangCap,
    int? NamKinhNghiem,
    string? TieuSu,
    string LoaiHopDong,
    string TrangThai,
    string TenChuyenKhoa)
{
    public static BacSiPublicResponse TuEntity(BacSi entity) => new(
        entity.IdBacSi,
        entity.IdChuyenKhoa,
        entity.HoTen,
        entity.AnhDaiDien,
        entity.BangCap,
        entity.NamKinhNghiem,
        entity.TieuSu,
        entity.LoaiHopDong.ToString(),
        entity.TrangThai.ToString(),
        entity.ChuyenKhoa.TenChuyenKhoa);
}
