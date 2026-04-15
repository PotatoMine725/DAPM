using ClinicBooking.Domain.Entities;

namespace ClinicBooking.Application.Abstractions.Security;

public interface ITokenService
{
    AccessTokenResult TaoAccessToken(TaiKhoan taiKhoan);
    RefreshTokenResult TaoRefreshToken();
}

public record AccessTokenResult(string Token, DateTime HetHan);

public record RefreshTokenResult(string Token, DateTime HetHan);
