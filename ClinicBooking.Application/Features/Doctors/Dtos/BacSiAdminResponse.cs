using BacSiEntity = ClinicBooking.Domain.Entities.BacSi;

namespace ClinicBooking.Application.Features.Doctors.Dtos;

public sealed record BacSiAdminResponse(
    int IdBacSi,
    int IdTaiKhoan,
    string TenDangNhap,
    string Email,
    string SoDienThoai,
    string HoTen,
    int IdChuyenKhoa,
    string TenChuyenKhoa,
    string LoaiHopDong,
    string TrangThai,
    string? AnhDaiDien,
    string? BangCap,
    int? NamKinhNghiem,
    string? TieuSu,
    bool TaiKhoanHoatDong,
    DateTime NgayTao)
{
    public static BacSiAdminResponse TuEntity(BacSiEntity entity) => new(
        entity.IdBacSi,
        entity.IdTaiKhoan,
        entity.TaiKhoan.TenDangNhap,
        entity.TaiKhoan.Email,
        entity.TaiKhoan.SoDienThoai,
        entity.HoTen,
        entity.IdChuyenKhoa,
        entity.ChuyenKhoa.TenChuyenKhoa,
        entity.LoaiHopDong.ToString(),
        entity.TrangThai.ToString(),
        entity.AnhDaiDien,
        entity.BangCap,
        entity.NamKinhNghiem,
        entity.TieuSu,
        entity.TaiKhoan.TrangThai,
        entity.NgayTao);
}
