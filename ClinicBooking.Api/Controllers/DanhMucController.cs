using ClinicBooking.Api.Contracts.DanhMuc;
using ClinicBooking.Application.Common.Constants;
using ClinicBooking.Application.Features.DanhMuc.Commands.CapNhatChuyenKhoa;
using ClinicBooking.Application.Features.DanhMuc.Commands.CapNhatDichVu;
using ClinicBooking.Application.Features.DanhMuc.Commands.TaoChuyenKhoa;
using ClinicBooking.Application.Features.DanhMuc.Commands.TaoDichVu;
using ClinicBooking.Application.Features.DanhMuc.Commands.XoaChuyenKhoa;
using ClinicBooking.Application.Features.DanhMuc.Commands.XoaDichVu;
using ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachChuyenKhoa;
using ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachDichVu;
using ClinicBooking.Application.Features.DanhMuc.Queries.LayChuyenKhoaById;
using ClinicBooking.Application.Features.DanhMuc.Queries.LayDichVuById;
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

    [HttpGet("chuyen-khoa")]
    [Authorize(Roles = $"{VaiTroConstants.Admin},{VaiTroConstants.LeTan},{VaiTroConstants.BacSi}")]
    [ProducesResponseType(typeof(IReadOnlyList<ChuyenKhoaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ChuyenKhoaDto>>> DanhSachChuyenKhoa(
        [FromQuery] int soTrang = 1,
        [FromQuery] int kichThuocTrang = 20,
        [FromQuery] bool? hienThi = null,
        [FromQuery] string? tuKhoa = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new DanhSachChuyenKhoaQuery(soTrang, kichThuocTrang, hienThi, tuKhoa),
            cancellationToken);

        return Ok(result.Select(x => x.TuDto()).ToList());
    }

    [HttpGet("chuyen-khoa/{idChuyenKhoa:int}")]
    [Authorize(Roles = $"{VaiTroConstants.Admin},{VaiTroConstants.LeTan},{VaiTroConstants.BacSi}")]
    [ProducesResponseType(typeof(ChuyenKhoaDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ChuyenKhoaDto>> LayChuyenKhoaById(
        int idChuyenKhoa,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new LayChuyenKhoaByIdQuery(idChuyenKhoa), cancellationToken);
        return Ok(result.TuDto());
    }

    [HttpPut("chuyen-khoa/{idChuyenKhoa:int}")]
    [Authorize(Roles = VaiTroConstants.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> CapNhatChuyenKhoa(
        int idChuyenKhoa,
        [FromBody] CapNhatChuyenKhoaRequest request,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new CapNhatChuyenKhoaCommand(
                idChuyenKhoa,
                request.TenChuyenKhoa,
                request.MoTa,
                request.ThoiGianSlotMacDinh,
                request.GioMoDatLich,
                request.GioDongDatLich,
                request.HienThi),
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("chuyen-khoa/{idChuyenKhoa:int}")]
    [Authorize(Roles = VaiTroConstants.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> XoaChuyenKhoa(
        int idChuyenKhoa,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(new XoaChuyenKhoaCommand(idChuyenKhoa), cancellationToken);
        return NoContent();
    }

    [HttpPost("dich-vu")]
    [Authorize(Roles = VaiTroConstants.Admin)]
    [ProducesResponseType(typeof(TaoDichVuResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<TaoDichVuResponse>> TaoDichVu(
        [FromBody] TaoDichVuRequest request,
        CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(
            new TaoDichVuCommand(
                request.IdChuyenKhoa,
                request.TenDichVu,
                request.MoTa,
                request.ThoiGianUocTinh,
                request.HienThi),
            cancellationToken);

        return StatusCode(StatusCodes.Status201Created, new TaoDichVuResponse(id));
    }

    [HttpGet("dich-vu")]
    [Authorize(Roles = $"{VaiTroConstants.Admin},{VaiTroConstants.LeTan},{VaiTroConstants.BacSi}")]
    [ProducesResponseType(typeof(IReadOnlyList<DichVuDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<DichVuDto>>> DanhSachDichVu(
        [FromQuery] int soTrang = 1,
        [FromQuery] int kichThuocTrang = 20,
        [FromQuery] int? idChuyenKhoa = null,
        [FromQuery] bool? hienThi = null,
        [FromQuery] string? tuKhoa = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new DanhSachDichVuQuery(soTrang, kichThuocTrang, idChuyenKhoa, hienThi, tuKhoa),
            cancellationToken);

        return Ok(result.Select(x => x.TuDto()).ToList());
    }

    [HttpGet("dich-vu/{idDichVu:int}")]
    [Authorize(Roles = $"{VaiTroConstants.Admin},{VaiTroConstants.LeTan},{VaiTroConstants.BacSi}")]
    [ProducesResponseType(typeof(DichVuDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<DichVuDto>> LayDichVuById(
        int idDichVu,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new LayDichVuByIdQuery(idDichVu), cancellationToken);
        return Ok(result.TuDto());
    }

    [HttpPut("dich-vu/{idDichVu:int}")]
    [Authorize(Roles = VaiTroConstants.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> CapNhatDichVu(
        int idDichVu,
        [FromBody] CapNhatDichVuRequest request,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new CapNhatDichVuCommand(
                idDichVu,
                request.IdChuyenKhoa,
                request.TenDichVu,
                request.MoTa,
                request.ThoiGianUocTinh,
                request.HienThi),
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("dich-vu/{idDichVu:int}")]
    [Authorize(Roles = VaiTroConstants.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> XoaDichVu(
        int idDichVu,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(new XoaDichVuCommand(idDichVu), cancellationToken);
        return NoContent();
    }
}
