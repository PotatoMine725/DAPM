using System.Security.Claims;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Constants;
using ClinicBooking.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace ClinicBooking.Infrastructure.Identity;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public bool DaXacThuc => User?.Identity?.IsAuthenticated ?? false;

    public int? IdTaiKhoan
    {
        get
        {
            var value = User?.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)
                ?? User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(value, out var id) ? id : null;
        }
    }

    public string? TenDangNhap =>
        User?.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.UniqueName)
        ?? User?.Identity?.Name;

    public VaiTro? VaiTro
    {
        get
        {
            var roleClaim = User?.FindFirstValue(ClaimTypes.Role);
            return roleClaim switch
            {
                VaiTroConstants.Admin => Domain.Enums.VaiTro.Admin,
                VaiTroConstants.LeTan => Domain.Enums.VaiTro.LeTan,
                VaiTroConstants.BacSi => Domain.Enums.VaiTro.BacSi,
                VaiTroConstants.BenhNhan => Domain.Enums.VaiTro.BenhNhan,
                _ => null
            };
        }
    }
}
