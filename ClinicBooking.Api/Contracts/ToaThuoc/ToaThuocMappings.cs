using ClinicBooking.Application.Features.ToaThuoc.Dtos;

namespace ClinicBooking.Api.Contracts.ToaThuoc;

public static class ToaThuocMappings
{
    public static ToaThuocDto TuDto(this ToaThuocResponse response) => new(
        response.IdToaThuoc,
        response.IdHoSoKham,
        response.IdThuoc,
        response.TenThuoc,
        response.LieuLuong,
        response.CachDung,
        response.SoNgayDung,
        response.GhiChu,
        response.NgayKham,
        response.MaLichHen);

    public static ToaThuocChiTietInput TuInput(this ToaThuocChiTietRequest request) => new(
        request.IdThuoc,
        request.LieuLuong,
        request.CachDung,
        request.SoNgayDung,
        request.GhiChu);
}
