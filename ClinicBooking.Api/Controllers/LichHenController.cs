using ClinicBooking.Api.Contracts.LichHen;
using ClinicBooking.Application.Common.Constants;
using ClinicBooking.Application.Features.HangCho.Dtos;
using ClinicBooking.Application.Features.LichHen.Commands.CheckInLichHen;
using ClinicBooking.Application.Features.LichHen.Commands.GiaiPhongGiuCho;
using ClinicBooking.Application.Features.LichHen.Commands.HuyLichHen;
using ClinicBooking.Application.Features.LichHen.Commands.XacNhanLichHen;
using ClinicBooking.Application.Features.LichHen.Dtos;
using ClinicBooking.Application.Features.LichHen.Queries.DanhSachLichHenCuaToi;
using ClinicBooking.Application.Features.LichHen.Queries.DanhSachLichHenTheoNgay;
using ClinicBooking.Application.Features.LichHen.Queries.XemLichHen;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicBooking.Api.Controllers;

[ApiController]
[Route("api/lich-hen")]
[Authorize]
public class LichHenController : ControllerBase
{
    private readonly IMediator _mediator;

    public LichHenController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("{idLichHen:int}/xac-nhan")]
    [Authorize(Roles = $"{VaiTroConstants.LeTan},{VaiTroConstants.Admin}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> XacNhan(int idLichHen, CancellationToken cancellationToken)
    {
        await _mediator.Send(new XacNhanLichHenCommand(idLichHen), cancellationToken);
        return Ok();
    }

    [HttpPost("{idLichHen:int}/huy")]
    [Authorize(Roles = $"{VaiTroConstants.BenhNhan},{VaiTroConstants.LeTan},{VaiTroConstants.Admin}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Huy(
        int idLichHen,
        [FromBody] HuyLichHenRequest request,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(new HuyLichHenCommand(idLichHen, request.LyDo), cancellationToken);
        return Ok();
    }

    [HttpPost("{idLichHen:int}/check-in")]
    [Authorize(Roles = VaiTroConstants.LeTan)]
    [ProducesResponseType(typeof(HangChoResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<HangChoResponse>> CheckIn(int idLichHen, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CheckInLichHenCommand(idLichHen), cancellationToken);
        return Ok(result);
    }

    [HttpPost("giu-cho/{idGiuCho:int}/giai-phong")]
    [Authorize(Roles = VaiTroConstants.LeTan)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GiaiPhongGiuCho(int idGiuCho, CancellationToken cancellationToken)
    {
        await _mediator.Send(new GiaiPhongGiuChoCommand(idGiuCho), cancellationToken);
        return NoContent();
    }

    [HttpGet("{idLichHen:int}")]
    [ProducesResponseType(typeof(LichHenResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<LichHenResponse>> Xem(int idLichHen, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new XemLichHenQuery(idLichHen), cancellationToken);
        return Ok(result);
    }

    [HttpGet("cua-toi")]
    [Authorize(Roles = VaiTroConstants.BenhNhan)]
    [ProducesResponseType(typeof(DanhSachLichHenResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<DanhSachLichHenResponse>> CuaToi(
        [FromQuery] DanhSachLichHenCuaToiRequest request,
        CancellationToken cancellationToken)
    {
        var query = new DanhSachLichHenCuaToiQuery(request.TrangThai, request.SoTrang, request.KichThuocTrang);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("theo-ngay")]
    [Authorize(Roles = $"{VaiTroConstants.LeTan},{VaiTroConstants.Admin}")]
    [ProducesResponseType(typeof(IReadOnlyList<LichHenTomTatResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<LichHenTomTatResponse>>> TheoNgay(
        [FromQuery] DateOnly ngay,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DanhSachLichHenTheoNgayQuery(ngay), cancellationToken);
        return Ok(result);
    }
}
