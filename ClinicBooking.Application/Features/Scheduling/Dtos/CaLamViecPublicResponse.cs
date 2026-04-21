using ClinicBooking.Domain.Entities;

namespace ClinicBooking.Application.Features.Scheduling.Dtos;

public sealed record CaLamViecPublicResponse(
    int IdCaLamViec,
    int IdBacSi,
    int IdPhong,
    int IdChuyenKhoa,
    int IdDinhNghiaCa,
    DateOnly NgayLamViec,
    TimeOnly GioBatDau,
    TimeOnly GioKetThuc,
    int ThoiGianSlot,
    int SoSlotToiDa,
    int SoSlotDaDat,
    string TrangThaiDuyet,
    string NguonTaoCa,
    string HoTenBacSi,
    string MaPhong,
    string TenChuyenKhoa,
    bool ConTrong)
{
    public static CaLamViecPublicResponse TuEntity(CaLamViec entity) => new(
        entity.IdCaLamViec,
        entity.IdBacSi,
        entity.IdPhong,
        entity.IdChuyenKhoa,
        entity.IdDinhNghiaCa,
        entity.NgayLamViec,
        entity.GioBatDau,
        entity.GioKetThuc,
        entity.ThoiGianSlot,
        entity.SoSlotToiDa,
        entity.SoSlotDaDat,
        entity.TrangThaiDuyet.ToString(),
        entity.NguonTaoCa.ToString(),
        entity.BacSi.HoTen,
        entity.Phong.MaPhong,
        entity.ChuyenKhoa.TenChuyenKhoa,
        entity.SoSlotDaDat < entity.SoSlotToiDa);
}
