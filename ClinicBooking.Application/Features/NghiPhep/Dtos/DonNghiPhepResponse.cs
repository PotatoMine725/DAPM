using ClinicBooking.Domain.Entities;
using ClinicBooking.Domain.Enums;

namespace ClinicBooking.Application.Features.NghiPhep.Dtos;

public sealed record DonNghiPhepResponse(
    int IdDonNghiPhep,
    int IdBacSi,
    int IdCaLamViec,
    string HoTenBacSi,
    string TenChuyenKhoa,
    DateOnly NgayLamViec,
    TimeOnly GioBatDau,
    TimeOnly GioKetThuc,
    LoaiNghiPhep LoaiNghiPhep,
    string LyDo,
    TrangThaiDuyetDon TrangThaiDuyet,
    DateTime NgayGuiDon,
    string? LyDoTuChoi,
    DateTime? NgayXuLy)
{
    public static DonNghiPhepResponse TuEntity(DonNghiPhep entity) => new(
        entity.IdDonNghiPhep,
        entity.IdBacSi,
        entity.IdCaLamViec,
        entity.BacSi.HoTen,
        entity.BacSi.ChuyenKhoa.TenChuyenKhoa,
        entity.CaLamViec.NgayLamViec,
        entity.CaLamViec.GioBatDau,
        entity.CaLamViec.GioKetThuc,
        entity.LoaiNghiPhep,
        entity.LyDo,
        entity.TrangThaiDuyet,
        entity.NgayGuiDon,
        entity.LyDoTuChoi,
        entity.NgayXuLy);
}
