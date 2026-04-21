using ClinicBooking.Api.Contracts.HoSoKham;
using ClinicBooking.Application.Common.Constants;
using ClinicBooking.Application.Features.HoSoKham.Commands.CapNhatHoSoKham;
using ClinicBooking.Application.Features.HoSoKham.Commands.TaoHoSoKham;
using ClinicBooking.Application.Features.HoSoKham.Queries.LayHoSoKhamById;
using ClinicBooking.Application.Features.HoSoKham.Queries.LichSuKhamCuaToi;
using ClinicBooking.Application.Features.HoSoKham.Queries.LichSuKhamTheoBenhNhan;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicBooking.Api.Controllers;

[ApiController]
[Route("api/ho-so-kham")]
[Authorize]
public class HoSoKhamController : ControllerBase
{
    private readonly IMediator _mediator;

    public HoSoKhamController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Roles = VaiTroConstants.BacSi)]
    [ProducesResponseType(typeof(TaoHoSoKhamResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<TaoHoSoKhamResponse>> TaoHoSoKham(
        [FromBody] TaoHoSoKhamRequest request,
        CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(
            new TaoHoSoKhamCommand(request.IdLichHen, request.ChanDoan, request.KetQuaKham, request.GhiChu),
            cancellationToken);

        return StatusCode(StatusCodes.Status201Created, new TaoHoSoKhamResponse(id));
    }

    [HttpPut("{idHoSoKham:int}")]
    [Authorize(Roles = VaiTroConstants.BacSi)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> CapNhatHoSoKham(
        int idHoSoKham,
        [FromBody] CapNhatHoSoKhamRequest request,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new CapNhatHoSoKhamCommand(idHoSoKham, request.ChanDoan, request.KetQuaKham, request.GhiChu),
            cancellationToken);

        return NoContent();
    }

    [HttpGet("{idHoSoKham:int}")]
    [Authorize(Roles = $"{VaiTroConstants.Admin},{VaiTroConstants.LeTan},{VaiTroConstants.BacSi},{VaiTroConstants.BenhNhan}")]
    [ProducesResponseType(typeof(HoSoKhamDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<HoSoKhamDto>> LayHoSoKhamById(int idHoSoKham, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new LayHoSoKhamByIdQuery(idHoSoKham), cancellationToken);
        return Ok(result.TuDto());
    }

    [HttpGet("benh-nhan/{idBenhNhan:int}")]
    [Authorize(Roles = $"{VaiTroConstants.Admin},{VaiTroConstants.LeTan},{VaiTroConstants.BacSi}")]
    [ProducesResponseType(typeof(IReadOnlyList<HoSoKhamTomTatDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<HoSoKhamTomTatDto>>> LichSuTheoBenhNhan(
        int idBenhNhan,
        [FromQuery] int soTrang = 1,
        [FromQuery] int kichThuocTrang = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new LichSuKhamTheoBenhNhanQuery(idBenhNhan, soTrang, kichThuocTrang),
            cancellationToken);

        return Ok(result.Select(x => x.TuDto()).ToList());
    }

    [HttpGet("cua-toi")]
    [Authorize(Roles = VaiTroConstants.BenhNhan)]
    [ProducesResponseType(typeof(IReadOnlyList<HoSoKhamTomTatDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<HoSoKhamTomTatDto>>> LichSuCuaToi(
        [FromQuery] int soTrang = 1,
        [FromQuery] int kichThuocTrang = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new LichSuKhamCuaToiQuery(soTrang, kichThuocTrang), cancellationToken);
        return Ok(result.Select(x => x.TuDto()).ToList());
    }
}
