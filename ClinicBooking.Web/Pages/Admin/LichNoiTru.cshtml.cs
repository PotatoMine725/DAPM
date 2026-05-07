using ClinicBooking.Application.Common.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicBooking.Web.Pages.Admin;

[Authorize(Roles = VaiTroConstants.Admin)]
public class LichNoiTruModel : PageModel
{
    public void OnGet()
    {
        // Implementation for inpatient schedule page
    }
}
