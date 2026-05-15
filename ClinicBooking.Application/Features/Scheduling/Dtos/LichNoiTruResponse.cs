using ClinicBooking.Domain.Entities;

namespace ClinicBooking.Application.Features.Scheduling.Dtos;

public sealed record LichNoiTruResponse(
    int IdLichNoiTru,
    int IdBacSi,
    string TenBacSi,
    int IdPhong,
    string TenPhong,
    string MaPhong,
    int IdDinhNghiaCa,
    string TenCa,
    TimeOnly GioBatDau,
    TimeOnly GioKetThuc,
    int NgayTrongTuan,
    DateOnly NgayApDung,
    DateOnly? NgayKetThuc,
    bool TrangThai)
{
    public static LichNoiTruResponse TuEntity(LichNoiTru entity) => new(
        entity.IdLichNoiTru,
        entity.IdBacSi,
        entity.BacSi.HoTen,
        entity.IdPhong,
        entity.Phong.TenPhong,
        entity.Phong.MaPhong,
        entity.IdDinhNghiaCa,
        entity.DinhNghiaCa.TenCa,
        entity.DinhNghiaCa.GioBatDauMacDinh,
        entity.DinhNghiaCa.GioKetThucMacDinh,
        entity.NgayTrongTuan,
        entity.NgayApDung,
        entity.NgayKetThuc,
        entity.TrangThai);
}
