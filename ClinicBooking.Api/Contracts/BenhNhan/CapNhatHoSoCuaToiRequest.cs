using ClinicBooking.Domain.Enums;

namespace ClinicBooking.Api.Contracts.BenhNhan;

public record CapNhatHoSoCuaToiRequest(
    string HoTen,
    DateOnly? NgaySinh,
    GioiTinh? GioiTinh,
    string? Cccd,
    string? DiaChi);
