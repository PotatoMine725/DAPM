using ClinicBooking.Application.Features.DanhMuc.Dtos;

namespace ClinicBooking.Api.Contracts.DanhMuc;

public static class CongKhaiMappings
{
    public static ChuyenKhoaCongKhaiDto TuDto(this ChuyenKhoaCongKhaiResponse response) => new(
        response.IdChuyenKhoa,
        response.TenChuyenKhoa,
        response.MoTa,
        response.ThoiGianSlotMacDinh,
        response.GioMoDatLich,
        response.GioDongDatLich);

    public static DichVuCongKhaiDto TuDto(this DichVuCongKhaiResponse response) => new(
        response.IdDichVu,
        response.IdChuyenKhoa,
        response.TenDichVu,
        response.MoTa,
        response.ThoiGianUocTinh,
        response.TenChuyenKhoa);
}
