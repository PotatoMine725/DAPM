namespace ClinicBooking.Application.Features.Auth.Dtos;

public record XacThucResponse(
    int IdTaiKhoan,
    string TenDangNhap,
    string Email,
    string VaiTro,
    string AccessToken,
    DateTime AccessTokenHetHan,
    string RefreshToken,
    DateTime RefreshTokenHetHan);
