using ClinicBooking.Api.Contracts.Thuoc;
using ClinicBooking.Application.Common.Constants;
using ClinicBooking.Application.Features.Thuoc.Commands.CapNhatThuoc;
using ClinicBooking.Application.Features.Thuoc.Commands.TaoThuoc;
using ClinicBooking.Application.Features.Thuoc.Commands.XoaThuoc;
using ClinicBooking.Application.Features.Thuoc.Queries.DanhSachThuoc;
using ClinicBooking.Application.Features.Thuoc.Queries.LayThuocById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicBooking.Api.Controllers;

[ApiController]
[Route("api/thuoc")]
[Authorize]
public class ThuocController : ControllerBase
{
    private readonly IMediator _mediator;

    public ThuocController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Roles = VaiTroConstants.Admin)]
    [ProducesResponseType(typeof(TaoThuocResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<TaoThuocResponse>> TaoThuoc(
        [FromBody] TaoThuocRequest request,
        CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(
            new TaoThuocCommand(request.TenThuoc, request.HoatChat, request.DonVi, request.GhiChu),
            cancellationToken);

        return StatusCode(StatusCodes.Status201Created, new TaoThuocResponse(id));
    }

    [HttpGet]
    [Authorize(Roles = $"{VaiTroConstants.Admin},{VaiTroConstants.BacSi}")]
    [ProducesResponseType(typeof(IReadOnlyList<ThuocDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ThuocDto>>> DanhSachThuoc(
        [FromQuery] int soTrang = 1,
        [FromQuery] int kichThuocTrang = 20,
        [FromQuery] string? tuKhoa = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new DanhSachThuocQuery(soTrang, kichThuocTrang, tuKhoa),
            cancellationToken);

        return Ok(result.Select(x => x.TuDto()).ToList());
    }

    [HttpGet("{idThuoc:int}")]
    [Authorize(Roles = $"{VaiTroConstants.Admin},{VaiTroConstants.BacSi}")]
    [ProducesResponseType(typeof(ThuocDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ThuocDto>> LayThuocById(int idThuoc, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new LayThuocByIdQuery(idThuoc), cancellationToken);
        return Ok(result.TuDto());
    }

    [HttpPut("{idThuoc:int}")]
    [Authorize(Roles = VaiTroConstants.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> CapNhatThuoc(
        int idThuoc,
        [FromBody] CapNhatThuocRequest request,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new CapNhatThuocCommand(idThuoc, request.TenThuoc, request.HoatChat, request.DonVi, request.GhiChu),
            cancellationToken);
        return NoContent();
    }

    [HttpDelete("{idThuoc:int}")]
    [Authorize(Roles = VaiTroConstants.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> XoaThuoc(int idThuoc, CancellationToken cancellationToken)
    {
        await _mediator.Send(new XoaThuocCommand(idThuoc), cancellationToken);
        return NoContent();
    }
}
