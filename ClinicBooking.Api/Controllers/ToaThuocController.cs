using ClinicBooking.Api.Contracts.ToaThuoc;
using ClinicBooking.Application.Common.Constants;
using ClinicBooking.Application.Features.ToaThuoc.Commands.CapNhatToaThuoc;
using ClinicBooking.Application.Features.ToaThuoc.Commands.TaoToaThuoc;
using ClinicBooking.Application.Features.ToaThuoc.Queries.LayToaCuaToi;
using ClinicBooking.Application.Features.ToaThuoc.Queries.LayToaTheoHoSoKham;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicBooking.Api.Controllers;

[ApiController]
[Route("api/toa-thuoc")]
[Authorize]
public class ToaThuocController : ControllerBase
{
    private readonly IMediator _mediator;

    public ToaThuocController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Roles = VaiTroConstants.BacSi)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> TaoToaThuoc(
        [FromBody] TaoToaThuocRequest request,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new TaoToaThuocCommand(
                request.IdHoSoKham,
                request.DanhSachThuoc.Select(x => x.TuInput()).ToList()),
            cancellationToken);

        return NoContent();
    }

    [HttpPut("ho-so-kham/{idHoSoKham:int}")]
    [Authorize(Roles = VaiTroConstants.BacSi)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> CapNhatToaThuoc(
        int idHoSoKham,
        [FromBody] CapNhatToaThuocRequest request,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new CapNhatToaThuocCommand(
                idHoSoKham,
                request.DanhSachThuoc.Select(x => x.TuInput()).ToList()),
            cancellationToken);

        return NoContent();
    }

    [HttpGet("ho-so-kham/{idHoSoKham:int}")]
    [Authorize(Roles = $"{VaiTroConstants.Admin},{VaiTroConstants.LeTan},{VaiTroConstants.BacSi},{VaiTroConstants.BenhNhan}")]
    [ProducesResponseType(typeof(IReadOnlyList<ToaThuocDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ToaThuocDto>>> LayToaTheoHoSoKham(
        int idHoSoKham,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new LayToaTheoHoSoKhamQuery(idHoSoKham), cancellationToken);
        return Ok(result.Select(x => x.TuDto()).ToList());
    }

    [HttpGet("cua-toi")]
    [Authorize(Roles = VaiTroConstants.BenhNhan)]
    [ProducesResponseType(typeof(IReadOnlyList<ToaThuocDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ToaThuocDto>>> LayToaCuaToi(
        [FromQuery] int soTrang = 1,
        [FromQuery] int kichThuocTrang = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new LayToaCuaToiQuery(soTrang, kichThuocTrang), cancellationToken);
        return Ok(result.Select(x => x.TuDto()).ToList());
    }
}
