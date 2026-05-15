using System.Security.Claims;
using ClinicBooking.Application.Common.Constants;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.Auth.Commands.GuiOtpKichHoatWalkIn;
using ClinicBooking.Application.Features.Auth.Commands.KichHoatTaiKhoanWalkIn;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicBooking.Web.Pages.Auth;

public class LienKetHoSoModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly ILogger<LienKetHoSoModel> _logger;

    public LienKetHoSoModel(IMediator mediator, ILogger<LienKetHoSoModel> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public int Buoc { get; set; } = 1;
    public string? ErrorMessage { get; set; }

    // Bước 1
    [BindProperty] public string SoDienThoai { get; set; } = string.Empty;
    [BindProperty] public string EmailNhan { get; set; } = string.Empty;

    // Bước 2
    [BindProperty] public string MaOtp { get; set; } = string.Empty;
    [BindProperty] public string? Cccd { get; set; }
    [BindProperty] public DateOnly? NgaySinh { get; set; }
    [BindProperty] public string? HoTen { get; set; }
    [BindProperty] public string TenDangNhap { get; set; } = string.Empty;
    [BindProperty] public string Email { get; set; } = string.Empty;
    [BindProperty] public string MatKhau { get; set; } = string.Empty;
    [BindProperty] public string XacNhanMatKhau { get; set; } = string.Empty;

    public IActionResult OnGet(string? sdt = null)
    {
        if (User.Identity?.IsAuthenticated == true) return RedirectToPage("/Index");
        if (!string.IsNullOrWhiteSpace(sdt)) SoDienThoai = sdt;
        Buoc = 1;
        return Page();
    }

    // Bước 1 → gửi OTP
    public async Task<IActionResult> OnPostGuiOtpAsync()
    {
        if (string.IsNullOrWhiteSpace(EmailNhan))
        {
            ErrorMessage = "Vui lòng nhập email để nhận mã OTP.";
            Buoc = 1;
            return Page();
        }

        try
        {
            var idTaiKhoan = await _mediator.Send(new GuiOtpKichHoatWalkInCommand(SoDienThoai.Trim(), EmailNhan.Trim()));
            TempData["WalkIn_IdTaiKhoan"] = idTaiKhoan;
            TempData["WalkIn_SoDienThoai"] = SoDienThoai.Trim();
            TempData["WalkIn_EmailNhan"] = EmailNhan.Trim();
            Buoc = 2;
            return Page();
        }
        catch (NotFoundException)
        {
            ErrorMessage = "Không tìm thấy hồ sơ vãng lai với số điện thoại này. Vui lòng kiểm tra lại hoặc đăng ký tài khoản mới.";
            Buoc = 1;
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Loi gui OTP kich hoat walk-in SDT={Sdt}", SoDienThoai);
            ErrorMessage = "Đã có lỗi xảy ra. Vui lòng thử lại.";
            Buoc = 1;
            return Page();
        }
    }

    // Bước 2 → kích hoạt tài khoản
    public async Task<IActionResult> OnPostKichHoatAsync()
    {
        // Restore IdTaiKhoan từ TempData
        var idTaiKhoan = TempData["WalkIn_IdTaiKhoan"] is int id ? id : (int?)null;
        TempData.Keep("WalkIn_IdTaiKhoan");
        TempData.Keep("WalkIn_SoDienThoai");
        TempData.Keep("WalkIn_EmailNhan");

        if (idTaiKhoan is null)
        {
            ErrorMessage = "Phiên làm việc đã hết hạn. Vui lòng bắt đầu lại.";
            Buoc = 1;
            return Page();
        }

        if (MatKhau != XacNhanMatKhau)
        {
            ErrorMessage = "Mật khẩu nhập lại không khớp.";
            Buoc = 2;
            return Page();
        }

        try
        {
            var result = await _mediator.Send(new KichHoatTaiKhoanWalkInCommand(
                idTaiKhoan.Value,
                MaOtp.Trim(),
                string.IsNullOrWhiteSpace(Cccd) ? null : Cccd.Trim(),
                NgaySinh,
                string.IsNullOrWhiteSpace(HoTen) ? null : HoTen.Trim(),
                TenDangNhap.Trim(),
                Email.Trim(),
                MatKhau));

            // Auto-login — cùng pattern với DangNhapModel
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, result.IdTaiKhoan.ToString()),
                new(ClaimTypes.Name,           result.TenDangNhap),
                new(ClaimTypes.Email,          result.Email),
                new(ClaimTypes.Role,           result.VaiTro),
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

            TempData["SuccessMessage"] = "Tài khoản đã được kích hoạt thành công! Chào mừng bạn.";
            return RedirectToPage("/BenhNhan/DanhSachLichHen");
        }
        catch (UnauthorizedAccessException ex)
        {
            ErrorMessage = ex.Message;
            Buoc = 2;
            return Page();
        }
        catch (ForbiddenException ex)
        {
            ErrorMessage = ex.Message;
            Buoc = 2;
            return Page();
        }
        catch (ConflictException ex)
        {
            ErrorMessage = ex.Message;
            Buoc = 2;
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Loi kich hoat walk-in IdTaiKhoan={Id}", idTaiKhoan);
            ErrorMessage = "Đã có lỗi xảy ra. Vui lòng thử lại.";
            Buoc = 2;
            return Page();
        }
    }
}
