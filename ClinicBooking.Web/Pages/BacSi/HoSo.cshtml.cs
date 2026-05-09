using ClinicBooking.Application.Features.BacSi.Dtos;
using ClinicBooking.Application.Features.BacSi.Queries.HoSoBacSi;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicBooking.Web.Pages.BacSi;

[Authorize(Roles = "admin,le_tan")]
public class HoSoModel : PageModel
{
    private readonly IMediator _mediator;

    public HoSoModel(IMediator mediator) => _mediator = mediator;

    public BacSiResponse? HoSo { get; private set; }

    public async Task OnGetAsync(int idBacSi)
    {
        HoSo = await _mediator.Send(new HoSoBacSiQuery(idBacSi));
    }
}
