using ClinicBooking.Application.Common.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicBooking.Web.Pages.Admin;

[Authorize(Roles = VaiTroConstants.Admin)]
public class BacSiModel : PageModel
{
    public void OnGet()
    {
        // Implementation for doctors page
    }
}
