using ClinicBooking.Api.Contracts.DanhMuc;
using ClinicBooking.Application.Common.Constants;
using ClinicBooking.Application.Features.DanhMuc.Commands.TaoChuyenKhoa;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicBooking.Api.Controllers;

[ApiController]
[Route("api/danh-muc")]
[Authorize]
public class DanhMucController : ControllerBase
{
    private readonly IMediator _mediator;

    public DanhMucController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("chuyen-khoa")]
    [Authorize(Roles = VaiTroConstants.Admin)]
    [ProducesResponseType(typeof(TaoChuyenKhoaResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<TaoChuyenKhoaResponse>> TaoChuyenKhoa(
        [FromBody] TaoChuyenKhoaRequest request,
        CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(
            new TaoChuyenKhoaCommand(
                request.TenChuyenKhoa,
                request.MoTa,
                request.ThoiGianSlotMacDinh,
                request.GioMoDatLich,
                request.GioDongDatLich,
                request.HienThi),
            cancellationToken);

        return StatusCode(StatusCodes.Status201Created, new TaoChuyenKhoaResponse(id));
    }
}
