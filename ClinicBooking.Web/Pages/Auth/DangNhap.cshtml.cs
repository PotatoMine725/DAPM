using System.Security.Claims;
using ClinicBooking.Application.Common.Constants;
using ClinicBooking.Application.Features.Auth.Commands.DangNhap;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicBooking.Web.Pages.Auth;

public class DangNhapInputModel
{
    public string TenDangNhapHoacEmail { get; set; } = string.Empty;
    public string MatKhau { get; set; } = string.Empty;
}

public class DangNhapModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly ILogger<DangNhapModel> _logger;

    public DangNhapModel(IMediator mediator, ILogger<DangNhapModel> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [BindProperty]
    public DangNhapInputModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public IActionResult OnGet()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectBasedOnRole();
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        try
        {
            var result = await _mediator.Send(
                new DangNhapCommand(Input.TenDangNhapHoacEmail, Input.MatKhau));

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, result.IdTaiKhoan.ToString()),
                new(ClaimTypes.Name,           result.TenDangNhap),
                new(ClaimTypes.Email,          result.Email),
                new(ClaimTypes.Role,           result.VaiTro),
                // Sub claim de CurrentUserService tim duoc IdTaiKhoan
                new(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub,
                    result.IdTaiKhoan.ToString()),
                new(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.UniqueName,
                    result.TenDangNhap),
            };

            var identity  = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties { IsPersistent = true });

            return RedirectBasedOnRole(result.VaiTro);
        }
        catch (UnauthorizedAccessException ex)
        {
            ErrorMessage = ex.Message;
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Loi khi xu ly dang nhap cho '{DinhDanh}'", Input.TenDangNhapHoacEmail);
            ErrorMessage = $"Đã có lỗi xảy ra. Vui lòng thử lại. [DEBUG] {ex.GetType().Name}: {ex.Message}";
            return Page();
        }
    }

    public IActionResult OnGetRegister()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectBasedOnRole();
        }

        return Page();
    }

    private IActionResult RedirectBasedOnRole(string? vaiTro = null)
    {
        vaiTro ??= User.FindFirstValue(ClaimTypes.Role);
        return vaiTro switch
        {
            VaiTroConstants.Admin    => RedirectToPage("/Admin/Dashboard"),
            VaiTroConstants.LeTan    => RedirectToPage("/LeTan/Dashboard"),
            VaiTroConstants.BacSi    => RedirectToPage("/BacSi/HangCho"),
            VaiTroConstants.BenhNhan => RedirectToPage("/BenhNhan/DanhSachLichHen"),
            _                        => RedirectToPage("/Auth/DangNhap")
        };
    }
}
