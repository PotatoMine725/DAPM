using ClinicBooking.Domain.Entities;

namespace ClinicBooking.Application.Features.Scheduling.Dtos;

public sealed record CaLamViecAdminResponse(
    int IdCaLamViec,
    int IdBacSi,
    string HoTenBacSi,
    string TenChuyenKhoa,
    int IdPhong,
    string MaPhong,
    string TenPhong,
    string TenCa,
    DateOnly NgayLamViec,
    TimeOnly GioBatDau,
    TimeOnly GioKetThuc,
    int SoSlotToiDa,
    int SoSlotDaDat,
    string TrangThaiDuyet,
    string NguonTaoCa,
    int? IdBacSiYeuCau,
    int? IdAdminDuyet,
    string? LyDoTuChoi,
    DateTime? NgayDuyet,
    DateTime NgayTao)
{
    public static CaLamViecAdminResponse TuEntity(CaLamViec entity) => new(
        entity.IdCaLamViec,
        entity.IdBacSi,
        entity.BacSi.HoTen,
        entity.ChuyenKhoa.TenChuyenKhoa,
        entity.IdPhong,
        entity.Phong.MaPhong,
        entity.Phong.TenPhong,
        entity.DinhNghiaCa.TenCa,
        entity.NgayLamViec,
        entity.GioBatDau,
        entity.GioKetThuc,
        entity.SoSlotToiDa,
        entity.SoSlotDaDat,
        entity.TrangThaiDuyet.ToString(),
        entity.NguonTaoCa.ToString(),
        entity.IdBacSiYeuCau,
        entity.IdAdminDuyet,
        entity.LyDoTuChoi,
        entity.NgayDuyet,
        entity.NgayTao);
}
