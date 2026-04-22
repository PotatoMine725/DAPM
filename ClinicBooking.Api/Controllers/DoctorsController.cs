using ClinicBooking.Api.Contracts.Doctors;
using ClinicBooking.Application.Common.Constants;
using ClinicBooking.Application.Features.Doctors.Queries.DanhSachBacSiCongKhai;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicBooking.Api.Controllers;

[ApiController]
[Route("api/bac-si")]
[Authorize]
public class BacSiController : ControllerBase
{
    private readonly IMediator _mediator;

    public BacSiController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("cong-khai")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<BacSiPublicDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<BacSiPublicDto>>> DanhSachBacSiCongKhai(
        [FromQuery] int soTrang = 1,
        [FromQuery] int kichThuocTrang = 20,
        [FromQuery] int? idChuyenKhoa = null,
        [FromQuery] bool? dangLamViec = null,
        [FromQuery] string? tuKhoa = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new DanhSachBacSiCongKhaiQuery(soTrang, kichThuocTrang, idChuyenKhoa, dangLamViec, tuKhoa),
            cancellationToken);

        return Ok(result.Select(x => x.TuDto()).ToList());
    }
}
