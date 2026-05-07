using ClinicBooking.Application.Common.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicBooking.Web.Pages.Admin;

[Authorize(Roles = VaiTroConstants.Admin)]
public class ThongKeModel : PageModel
{
    public void OnGet()
    {
        // Implementation for statistics page
    }
}
