using ClinicBooking.Application.Features.DanhMuc.Dtos;

namespace ClinicBooking.Api.Contracts.DanhMuc;

public static class DichVuMappings
{
    public static DichVuDto TuDto(this DichVuResponse response) => new(
        response.IdDichVu,
        response.IdChuyenKhoa,
        response.TenDichVu,
        response.MoTa,
        response.ThoiGianUocTinh,
        response.HienThi,
        response.NgayTao,
        response.TenChuyenKhoa);
}
