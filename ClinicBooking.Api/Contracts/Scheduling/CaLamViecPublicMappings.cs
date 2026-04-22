using ClinicBooking.Application.Features.Scheduling.Dtos;

namespace ClinicBooking.Api.Contracts.Scheduling;

public static class CaLamViecPublicMappings
{
    public static CaLamViecPublicDto TuDto(this CaLamViecPublicResponse response) => new(
        response.IdCaLamViec,
        response.IdBacSi,
        response.IdPhong,
        response.IdChuyenKhoa,
        response.IdDinhNghiaCa,
        response.NgayLamViec,
        response.GioBatDau,
        response.GioKetThuc,
        response.ThoiGianSlot,
        response.SoSlotToiDa,
        response.SoSlotDaDat,
        response.TrangThaiDuyet,
        response.NguonTaoCa,
        response.HoTenBacSi,
        response.MaPhong,
        response.TenChuyenKhoa,
        response.ConTrong);
}
