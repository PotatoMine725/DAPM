using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Constants;
using ClinicBooking.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ClinicBooking.Infrastructure.Security;

public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly IDateTimeProvider _dateTimeProvider;

    public TokenService(
        IOptions<JwtSettings> jwtOptions,
        IDateTimeProvider dateTimeProvider)
    {
        _jwtSettings = jwtOptions.Value;
        _dateTimeProvider = dateTimeProvider;
    }

    public AccessTokenResult TaoAccessToken(TaiKhoan taiKhoan)
    {
        var now = _dateTimeProvider.UtcNow;
        var hetHan = now.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, taiKhoan.IdTaiKhoan.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.UniqueName, taiKhoan.TenDangNhap),
            new(JwtRegisteredClaimNames.Email, taiKhoan.Email),
            new(ClaimTypes.Role, taiKhoan.VaiTro.ToRoleClaim())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            notBefore: now,
            expires: hetHan,
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return new AccessTokenResult(tokenString, hetHan);
    }

    public RefreshTokenResult TaoRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        var token = Convert.ToBase64String(randomBytes);
        var hetHan = _dateTimeProvider.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);
        return new RefreshTokenResult(token, hetHan);
    }
}
