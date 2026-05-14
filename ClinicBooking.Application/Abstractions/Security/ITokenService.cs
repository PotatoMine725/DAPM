using System.Security.Claims;
using ClinicBooking.Domain.Entities;

namespace ClinicBooking.Application.Abstractions.Security;

public interface ITokenService
{
    AccessTokenResult TaoAccessToken(TaiKhoan taiKhoan, IEnumerable<Claim>? claimsThem = null);
    RefreshTokenResult TaoRefreshToken();
}

public record AccessTokenResult(string Token, DateTime HetHan);

public record RefreshTokenResult(string Token, DateTime HetHan);
