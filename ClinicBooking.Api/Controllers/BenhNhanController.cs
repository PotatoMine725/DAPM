using ClinicBooking.Api.Contracts.BenhNhan;
using ClinicBooking.Application.Common.Constants;
using ClinicBooking.Application.Features.BenhNhan.Commands.CapNhatHoSoCuaToi;
using ClinicBooking.Application.Features.BenhNhan.Commands.TaoBenhNhanWalkIn;
using ClinicBooking.Application.Features.BenhNhan.Queries.DanhSachBenhNhan;
using ClinicBooking.Application.Features.BenhNhan.Queries.LayBenhNhanById;
using ClinicBooking.Application.Features.BenhNhan.Queries.LayHoSoCuaToi;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicBooking.Api.Controllers;

[ApiController]
[Route("api/benh-nhan")]
[Authorize]
public class BenhNhanController : ControllerBase
{
    private readonly IMediator _mediator;

    public BenhNhanController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("ho-so-cua-toi")]
    [Authorize(Roles = VaiTroConstants.BenhNhan)]
    [ProducesResponseType(typeof(BenhNhanDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<BenhNhanDto>> LayHoSoCuaToi(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new LayHoSoCuaToiQuery(), cancellationToken);
        return Ok(result.TuDto());
    }

    [HttpPut("ho-so-cua-toi")]
    [Authorize(Roles = VaiTroConstants.BenhNhan)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> CapNhatHoSoCuaToi(
        [FromBody] CapNhatHoSoCuaToiRequest request,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new CapNhatHoSoCuaToiCommand(
                request.HoTen,
                request.NgaySinh,
                request.GioiTinh,
                request.Cccd,
                request.DiaChi),
            cancellationToken);

        return NoContent();
    }

    [HttpGet]
    [Authorize(Roles = $"{VaiTroConstants.LeTan},{VaiTroConstants.Admin}")]
    [ProducesResponseType(typeof(IReadOnlyList<BenhNhanDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<BenhNhanDto>>> DanhSachBenhNhan(
        [FromQuery] int soTrang = 1,
        [FromQuery] int kichThuocTrang = 20,
        [FromQuery] bool? biHanChe = null,
        [FromQuery] string? tuKhoa = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new DanhSachBenhNhanQuery(soTrang, kichThuocTrang, biHanChe, tuKhoa),
            cancellationToken);

        return Ok(result.Select(x => x.TuDto()).ToList());
    }

    [HttpGet("{idBenhNhan:int}")]
    [Authorize(Roles = $"{VaiTroConstants.LeTan},{VaiTroConstants.Admin}")]
    [ProducesResponseType(typeof(BenhNhanDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<BenhNhanDto>> LayBenhNhanById(int idBenhNhan, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new LayBenhNhanByIdQuery(idBenhNhan), cancellationToken);
        return Ok(result.TuDto());
    }

    [HttpPost("walk-in")]
    [Authorize(Roles = $"{VaiTroConstants.LeTan},{VaiTroConstants.Admin}")]
    [ProducesResponseType(typeof(TaoBenhNhanWalkInResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<TaoBenhNhanWalkInResponse>> TaoBenhNhanWalkIn(
        [FromBody] TaoBenhNhanWalkInRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new TaoBenhNhanWalkInCommand(
                request.HoTen,
                request.SoDienThoai,
                request.NgaySinh,
                request.GioiTinh,
                request.Cccd,
                request.DiaChi),
            cancellationToken);

        return StatusCode(StatusCodes.Status201Created, new TaoBenhNhanWalkInResponse(result.IdBenhNhan, result.IdTaiKhoan));
    }
}
