using ClinicBooking.Api.Contracts.Auth;
using ClinicBooking.Application.Features.Auth.Commands.DangKy;
using ClinicBooking.Application.Features.Auth.Commands.DangNhap;
using ClinicBooking.Application.Features.Auth.Commands.DangXuat;
using ClinicBooking.Application.Features.Auth.Commands.LamMoiToken;
using ClinicBooking.Application.Features.Auth.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicBooking.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("dang-ky")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(XacThucResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<XacThucResponse>> DangKy(
        [FromBody] DangKyRequest request,
        CancellationToken cancellationToken)
    {
        var command = new DangKyCommand(
            request.TenDangNhap,
            request.Email,
            request.SoDienThoai,
            request.MatKhau,
            request.HoTen,
            request.NgaySinh,
            request.GioiTinh,
            request.Cccd,
            request.DiaChi);

        var result = await _mediator.Send(command, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPost("dang-nhap")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(XacThucResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<XacThucResponse>> DangNhap(
        [FromBody] DangNhapRequest request,
        CancellationToken cancellationToken)
    {
        var command = new DangNhapCommand(request.TenDangNhapHoacEmail, request.MatKhau);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPost("lam-moi-token")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(XacThucResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<XacThucResponse>> LamMoiToken(
        [FromBody] TokenRequest request,
        CancellationToken cancellationToken)
    {
        var command = new LamMoiTokenCommand(request.RefreshToken);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPost("dang-xuat")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DangXuat(
        [FromBody] TokenRequest request,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(new DangXuatCommand(request.RefreshToken), cancellationToken);
        return NoContent();
    }
}
