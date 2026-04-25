using BacSiEntity = ClinicBooking.Domain.Entities.BacSi;

namespace ClinicBooking.Application.Features.BacSi.Dtos;

public sealed record BacSiResponse(
    int IdBacSi,
    int IdTaiKhoan,
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
    /// <remarks>Caller must eager-load ChuyenKhoa navigation.</remarks>
    public static BacSiResponse TuEntity(BacSiEntity entity) => new(
        entity.IdBacSi,
        entity.IdTaiKhoan,
        entity.IdChuyenKhoa,
        entity.HoTen,
        entity.AnhDaiDien,
        entity.BangCap,
        entity.NamKinhNghiem,
        entity.TieuSu,
        entity.LoaiHopDong.ToString(),
        entity.TrangThai.ToString(),
        entity.ChuyenKhoa?.TenChuyenKhoa ?? string.Empty);
}
