using ClinicBooking.Api.Contracts.Scheduling;
using ClinicBooking.Application.Features.Scheduling.Queries.DanhSachCaLamViecCongKhai;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicBooking.Api.Controllers;

[ApiController]
[Route("api/ca-lam-viec")]
[Authorize]
public class CaLamViecController : ControllerBase
{
    private readonly IMediator _mediator;

    public CaLamViecController(IMediator mediator)
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
}
