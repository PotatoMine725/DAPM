using ClinicBooking.Api.Contracts.BacSi;
using ClinicBooking.Application.Common.Constants;
using ClinicBooking.Application.Features.BacSi.Commands.CapNhatBacSi;
using ClinicBooking.Application.Features.BacSi.Commands.TaoBacSi;
using ClinicBooking.Application.Features.BacSi.Commands.XoaBacSi;
using ClinicBooking.Application.Features.BacSi.Queries.DanhSachBacSi;
using ClinicBooking.Application.Features.BacSi.Queries.HoSoBacSi;
using ClinicBooking.Application.Features.BacSi.Queries.LayHoSoCuaToi;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicBooking.Api.Controllers;

[ApiController]
[Route("api/bac-si")]
public class BacSiController : ControllerBase
{
    private readonly IMediator _mediator;

    public BacSiController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Admin and le_tan can create doctor profiles.
    [HttpPost]
    [Authorize(Roles = $"{VaiTroConstants.Admin},{VaiTroConstants.LeTan}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    public async Task<ActionResult<int>> TaoBacSi([FromBody] TaoBacSiCommand request, CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, id);
    }

    // Only bac_si can read their own self-service profile.
    [HttpGet("ho-so-cua-toi")]
    [Authorize(Roles = VaiTroConstants.BacSi)]
    [ProducesResponseType(typeof(BacSiProfileDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<BacSiProfileDto>> LayHoSoCuaToi(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new LayHoSoCuaToiQuery(), cancellationToken);
        return Ok(result.TuProfileDto());
    }

    // Admin and le_tan can browse doctor directory.
    [HttpGet]
    [Authorize(Roles = $"{VaiTroConstants.Admin},{VaiTroConstants.LeTan}")]
    [ProducesResponseType(typeof(IReadOnlyList<BacSiDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<BacSiDto>>> DanhSachBacSi(
        [FromQuery] int soTrang = 1,
        [FromQuery] int kichThuocTrang = 20,
        [FromQuery] int? idChuyenKhoa = null,
        [FromQuery] string? tuKhoa = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new DanhSachBacSiQuery(soTrang, kichThuocTrang, idChuyenKhoa, tuKhoa), cancellationToken);
        return Ok(result.Select(x => x.TuDto()).ToList());
    }

    // Admin and le_tan can inspect an internal doctor profile by id.
    [HttpGet("ho-so/{idBacSi:int}")]
    [Authorize(Roles = $"{VaiTroConstants.Admin},{VaiTroConstants.LeTan}")]
    [ProducesResponseType(typeof(BacSiDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<BacSiDto>> HoSoBacSi(int idBacSi, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new HoSoBacSiQuery(idBacSi), cancellationToken);
        return Ok(result.TuDto());
    }

    // Admin and le_tan can update doctor profiles.
    [HttpPut("{idBacSi:int}")]
    [Authorize(Roles = $"{VaiTroConstants.Admin},{VaiTroConstants.LeTan}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> CapNhatBacSi(int idBacSi, [FromBody] CapNhatBacSiCommand request, CancellationToken cancellationToken)
    {
        await _mediator.Send(request with { IdBacSi = idBacSi }, cancellationToken);
        return NoContent();
    }

    // Admin and le_tan can soft-deactivate doctor profiles.
    [HttpDelete("{idBacSi:int}")]
    [Authorize(Roles = $"{VaiTroConstants.Admin},{VaiTroConstants.LeTan}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> XoaBacSi(int idBacSi, CancellationToken cancellationToken)
    {
        await _mediator.Send(new XoaBacSiCommand(idBacSi), cancellationToken);
        return NoContent();
    }
}
