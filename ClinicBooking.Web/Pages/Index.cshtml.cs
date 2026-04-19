using ClinicBooking.Application.Common.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace ClinicBooking.Web.Pages;

public class IndexModel : PageModel
{
    public IActionResult OnGet()
    {
        // Chua dang nhap -> ve trang dang nhap
        if (!(User.Identity?.IsAuthenticated ?? false))
        {
            return RedirectToPage("/Auth/DangNhap");
        }

        // Da dang nhap -> redirect theo vai tro
        var vaiTro = User.FindFirst(ClaimTypes.Role)?.Value;
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
