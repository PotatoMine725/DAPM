using ClinicBooking.Application.Features.BenhNhan.Commands.CapNhatHoSoCuaToi;
using ClinicBooking.Application.Features.BenhNhan.Dtos;
using ClinicBooking.Application.Features.BenhNhan.Queries.LayHoSoCuaToi;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicBooking.Web.Pages.BenhNhan;

[Authorize(Roles = "benh_nhan")]
public class HoSoCaNhanModel : PageModel
{
    private readonly IMediator _mediator;

    public HoSoCaNhanModel(IMediator mediator) => _mediator = mediator;

    // Hien thi
    public BenhNhanResponse? HoSo { get; private set; }

    // Form binding
    [BindProperty] public string HoTen { get; set; } = string.Empty;
    [BindProperty] public DateOnly? NgaySinh { get; set; }
    [BindProperty] public GioiTinh? GioiTinh { get; set; }
    [BindProperty] public string? Cccd { get; set; }
    [BindProperty] public string? DiaChi { get; set; }
    [BindProperty] public string Email { get; set; } = string.Empty;
    [BindProperty] public string SoDienThoai { get; set; } = string.Empty;

    public async Task OnGetAsync()
    {
        try
        {
            HoSo = await _mediator.Send(new LayHoSoCuaToiQuery());
            // Pre-fill form
            HoTen = HoSo.HoTen;
            NgaySinh = HoSo.NgaySinh;
            GioiTinh = HoSo.GioiTinh;
            Cccd = HoSo.Cccd;
            DiaChi = HoSo.DiaChi;
            Email = HoSo.Email;
            SoDienThoai = HoSo.SoDienThoai;
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await OnGetAsync();
            return Page();
        }

        try
        {
            await _mediator.Send(new CapNhatHoSoCuaToiCommand(
                HoTen,
                NgaySinh,
                GioiTinh,
                Cccd,
                DiaChi));

            if (HoSo is not null)
            {
                HoSo = HoSo with { Email = Email, SoDienThoai = SoDienThoai };
            }

            TempData["SuccessMessage"] = "Cập nhật hồ sơ thành công.";
            return RedirectToPage();
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            await OnGetAsync();
            return Page();
        }
    }
}
