using ClinicBooking.Domain.Enums;

namespace ClinicBooking.Api.Contracts.Auth;

public record DangKyRequest(
    string TenDangNhap,
    string Email,
    string SoDienThoai,
    string MatKhau,
    string HoTen,
    DateOnly? NgaySinh,
    GioiTinh? GioiTinh,
    string? Cccd,
    string? DiaChi);
