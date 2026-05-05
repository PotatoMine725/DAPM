using ClinicBooking.Application.Common.Constants;
using ClinicBooking.Application.Features.NghiPhep.Commands.DuyetDonNghiPhep;
using ClinicBooking.Application.Features.NghiPhep.Commands.NopDonNghiPhep;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicBooking.Api.Controllers;

[ApiController]
[Route("api/nghi-phep")]
[Authorize]
public class NghiPhepController : ControllerBase
{
    private readonly IMediator _mediator;

    public NghiPhepController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Roles = VaiTroConstants.BacSi)]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    public async Task<ActionResult<int>> NopDonNghiPhep([FromBody] NopDonNghiPhepCommand request, CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, id);
    }

    [HttpPut("{idDonNghiPhep:int}/duyet")]
    [Authorize(Roles = VaiTroConstants.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DuyetDonNghiPhep(int idDonNghiPhep, [FromQuery] int idNguoiDuyet, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DuyetDonNghiPhepCommand(idDonNghiPhep, idNguoiDuyet), cancellationToken);
        return NoContent();
    }
}
