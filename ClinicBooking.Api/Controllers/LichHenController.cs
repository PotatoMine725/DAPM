using ClinicBooking.Api.Contracts.LichHen;
using ClinicBooking.Application.Common.Constants;
using ClinicBooking.Application.Features.HangCho.Dtos;
using ClinicBooking.Application.Features.LichHen.Commands.CheckInLichHen;
using ClinicBooking.Application.Features.LichHen.Commands.DoiLichHen;
using ClinicBooking.Application.Features.LichHen.Commands.GiaiPhongGiuCho;
using ClinicBooking.Application.Features.LichHen.Commands.HuyLichHen;
using ClinicBooking.Application.Features.LichHen.Commands.TaoGiuCho;
using ClinicBooking.Application.Features.LichHen.Commands.TaoLichHen;
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

    [HttpPost("tao-lich-hen")]
    [Authorize(Roles = $"{VaiTroConstants.BenhNhan},{VaiTroConstants.LeTan},{VaiTroConstants.Admin}")]
    [ProducesResponseType(typeof(LichHenResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<LichHenResponse>> Tao(
        [FromBody] TaoLichHenRequest request,
        CancellationToken cancellationToken)
    {
        var command = new TaoLichHenCommand(
            request.IdCaLamViec,
            request.IdDichVu,
            request.IdBenhNhan,
            request.IdBacSiMongMuon,
            request.BacSiMongMuonNote,
            request.TrieuChung);
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(Xem), new { idLichHen = result.IdLichHen }, result);
    }

    [HttpPost("{idLichHen:int}/doi-lich")]
    [Authorize(Roles = $"{VaiTroConstants.BenhNhan},{VaiTroConstants.LeTan},{VaiTroConstants.Admin}")]
    [ProducesResponseType(typeof(LichHenResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<LichHenResponse>> DoiLich(
        int idLichHen,
        [FromBody] DoiLichHenRequest request,
        CancellationToken cancellationToken)
    {
        var command = new DoiLichHenCommand(
            idLichHen,
            request.IdCaLamViecMoi,
            request.IdDichVuMoi,
            request.IdBacSiMongMuon,
            request.BacSiMongMuonNote,
            request.TrieuChung,
            request.LyDo);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPost("tao-giu-cho")]
    [Authorize(Roles = $"{VaiTroConstants.LeTan},{VaiTroConstants.Admin}")]
    [ProducesResponseType(typeof(GiuChoResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<GiuChoResponse>> TaoGiuCho(
        [FromBody] TaoGiuChoRequest request,
        CancellationToken cancellationToken)
    {
        var command = new TaoGiuChoCommand(request.IdCaLamViec, request.IdBenhNhan);
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GiaiPhongGiuCho), new { idGiuCho = result.IdGiuCho }, result);
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
