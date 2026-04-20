using ClinicBooking.Application.Features.DanhMuc.Dtos;

namespace ClinicBooking.Api.Contracts.DanhMuc;

public static class ChuyenKhoaMappings
{
    public static ChuyenKhoaDto TuDto(this ChuyenKhoaResponse response) => new(
        response.IdChuyenKhoa,
        response.TenChuyenKhoa,
        response.MoTa,
        response.ThoiGianSlotMacDinh,
        response.GioMoDatLich,
        response.GioDongDatLich,
        response.HienThi);
}
