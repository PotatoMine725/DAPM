using ClinicBooking.Application.Features.HangCho.Dtos;

namespace ClinicBooking.Api.Contracts.HangCho;

public static class ThuTuHangChoMappings
{
    public static ThuTuHangChoDto ToDto(this ThuTuHangChoResponse response)
        => new(
            response.IdCaLamViec,
            response.IdBenhNhan,
            response.IdHangCho,
            response.SoThuTu,
            response.DaCoTrongHangCho,
            response.NgayCheckIn);
}
