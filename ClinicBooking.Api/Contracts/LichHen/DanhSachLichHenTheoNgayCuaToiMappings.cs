using ClinicBooking.Application.Features.LichHen.Dtos;

namespace ClinicBooking.Api.Contracts.LichHen;

public static class DanhSachLichHenTheoNgayCuaToiMappings
{
    public static IReadOnlyList<LichHenTomTatResponse> TuDto(this IReadOnlyList<LichHenTomTatResponse> items) => items;
}
