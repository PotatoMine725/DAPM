using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.Auth.Commands.DangKy;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace ClinicBooking.Web.Pages.Auth;

public class DangKyInputModel
{
    [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
    [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự.")]
    public string HoTen { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên đăng nhập từ 3 đến 50 ký tự.")]
    public string TenDangNhap { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập email.")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
    public string SoDienThoai { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự.")]
    [DataType(DataType.Password)]
    public string MatKhau { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập lại mật khẩu.")]
    [Compare(nameof(MatKhau), ErrorMessage = "Mật khẩu nhập lại không khớp.")]
    [DataType(DataType.Password)]
    public string XacNhanMatKhau { get; set; } = string.Empty;

    public DateOnly? NgaySinh { get; set; }
    public GioiTinh? GioiTinh { get; set; }

    [StringLength(20)]
    public string? Cccd { get; set; }

    [StringLength(255)]
    public string? DiaChi { get; set; }
}

public class DangKyModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly ILogger<DangKyModel> _logger;

    public DangKyModel(IMediator mediator, ILogger<DangKyModel> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [BindProperty]
    public DangKyInputModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    public IActionResult OnGet()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToPage("/Index");
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        try
        {
            await _mediator.Send(new DangKyCommand(
                Input.TenDangNhap.Trim(),
                Input.Email.Trim(),
                Input.SoDienThoai.Trim(),
                Input.MatKhau,
                Input.HoTen.Trim(),
                Input.NgaySinh,
                Input.GioiTinh,
                string.IsNullOrWhiteSpace(Input.Cccd) ? null : Input.Cccd.Trim(),
                string.IsNullOrWhiteSpace(Input.DiaChi) ? null : Input.DiaChi.Trim()));

            TempData["SuccessMessage"] = "Đăng ký thành công. Vui lòng đăng nhập.";
            return RedirectToPage("/Auth/DangNhap");
        }
        catch (ConflictException ex)
        {
            ErrorMessage = ex.Message;
            return Page();
        }
        catch (ValidationException ex)
        {
            ErrorMessage = ex.Message;
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Loi khi dang ky tai khoan '{Ten}'", Input.TenDangNhap);
            ErrorMessage = "Đã có lỗi xảy ra. Vui lòng thử lại. [DEBUG] " + ex.GetType().Name + ": " + ex.Message;
            return Page();
        }
    }
}
