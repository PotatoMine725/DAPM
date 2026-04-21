using ClinicBooking.Domain.Enums;

namespace ClinicBooking.Api.Contracts.BenhNhan;

public record TaoBenhNhanWalkInRequest(
    string HoTen,
    string SoDienThoai,
    DateOnly? NgaySinh,
    GioiTinh? GioiTinh,
    string? Cccd,
    string? DiaChi);
