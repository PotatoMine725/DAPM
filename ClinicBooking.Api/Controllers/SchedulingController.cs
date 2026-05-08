using ClinicBooking.Api.Contracts.Scheduling;
using ClinicBooking.Application.Common.Constants;
using ClinicBooking.Application.Features.Scheduling.Commands.DangKyCaLamViec;
using ClinicBooking.Application.Features.Scheduling.Commands.DuyetCaLamViec;
using ClinicBooking.Application.Features.Scheduling.Commands.TaoCaLamViec;
using ClinicBooking.Application.Features.Scheduling.Commands.XoaCaLamViec;
using ClinicBooking.Application.Features.Scheduling.Queries.DanhSachCaLamViecCongKhai;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicBooking.Api.Controllers;

[ApiController]
[Route("api/lich-lam-viec")]
[Authorize]
public sealed class SchedulingController : ControllerBase
{
    private readonly IMediator _mediator;

    public SchedulingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<CaLamViecPublicDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CaLamViecPublicDto>>> DanhSachCaLamViecCongKhai(
        [FromQuery] int soTrang = 1,
        [FromQuery] int kichThuocTrang = 20,
        [FromQuery] int? idBacSi = null,
        [FromQuery] int? idChuyenKhoa = null,
        [FromQuery] int? idPhong = null,
        [FromQuery] DateOnly? tuNgay = null,
        [FromQuery] DateOnly? denNgay = null,
        [FromQuery] bool? conTrong = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new DanhSachCaLamViecCongKhaiQuery(
                soTrang,
                kichThuocTrang,
                idBacSi,
                idChuyenKhoa,
                idPhong,
                tuNgay,
                denNgay,
                conTrong),
            cancellationToken);

        return Ok(result.Select(x => x.TuDto()).ToList());
    }

    [HttpPost]
    [Authorize(Roles = $"{VaiTroConstants.Admin},{VaiTroConstants.BacSi}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    public async Task<ActionResult<int>> TaoCaLamViec([FromBody] TaoCaLamViecCommand request, CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, id);
    }

    [HttpPut("{idCaLamViec:int}/duyet")]
    [Authorize(Roles = VaiTroConstants.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DuyetCaLamViec(int idCaLamViec, [FromBody] DuyetCaLamViecCommand request, CancellationToken cancellationToken)
    {
        await _mediator.Send(request with { IdCaLamViec = idCaLamViec }, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{idCaLamViec:int}")]
    [Authorize(Roles = VaiTroConstants.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> XoaCaLamViec(int idCaLamViec, CancellationToken cancellationToken)
    {
        await _mediator.Send(new XoaCaLamViecCommand(idCaLamViec), cancellationToken);
        return NoContent();
    }

    [HttpPost("{idCaLamViec:int}/dang-ky")]
    [Authorize(Roles = VaiTroConstants.BacSi)]
    [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
    public async Task<ActionResult<int>> DangKyCaLamViec(int idCaLamViec, CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(new DangKyCaLamViecCommand(idCaLamViec), cancellationToken);
        return StatusCode(StatusCodes.Status201Created, id);
    }
}
