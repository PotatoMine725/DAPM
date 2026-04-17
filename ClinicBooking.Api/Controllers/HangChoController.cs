using ClinicBooking.Application.Common.Constants;
using ClinicBooking.Application.Features.HangCho.Commands.GoiBenhNhanKeTiep;
using ClinicBooking.Application.Features.HangCho.Commands.HoanThanhLuotKham;
using ClinicBooking.Application.Features.HangCho.Dtos;
using ClinicBooking.Application.Features.HangCho.Queries.XemHangChoTheoCa;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicBooking.Api.Controllers;

[ApiController]
[Route("api/hang-cho")]
[Authorize]
public class HangChoController : ControllerBase
{
    private readonly IMediator _mediator;

    public HangChoController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("goi-ke-tiep/{idCaLamViec:int}")]
    [Authorize(Roles = $"{VaiTroConstants.BacSi},{VaiTroConstants.LeTan}")]
    [ProducesResponseType(typeof(HangChoResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<HangChoResponse>> GoiKeTiep(int idCaLamViec, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GoiBenhNhanKeTiepCommand(idCaLamViec), cancellationToken);
        return Ok(result);
    }

    [HttpPost("{idHangCho:int}/hoan-thanh")]
    [Authorize(Roles = VaiTroConstants.BacSi)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> HoanThanh(int idHangCho, CancellationToken cancellationToken)
    {
        await _mediator.Send(new HoanThanhLuotKhamCommand(idHangCho), cancellationToken);
        return Ok();
    }

    [HttpGet("theo-ca/{idCaLamViec:int}")]
    [Authorize(Roles = $"{VaiTroConstants.LeTan},{VaiTroConstants.BacSi},{VaiTroConstants.Admin}")]
    [ProducesResponseType(typeof(IReadOnlyList<HangChoResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<HangChoResponse>>> TheoCa(int idCaLamViec, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new XemHangChoTheoCaQuery(idCaLamViec), cancellationToken);
        return Ok(result);
    }
}
