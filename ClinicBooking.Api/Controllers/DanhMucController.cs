using ClinicBooking.Api.Contracts.DanhMuc;
using ClinicBooking.Application.Common.Constants;
using ClinicBooking.Application.Features.DanhMuc.Commands.CapNhatChuyenKhoa;
using ClinicBooking.Application.Features.DanhMuc.Commands.CapNhatDinhNghiaCa;
using ClinicBooking.Application.Features.DanhMuc.Commands.CapNhatDichVu;
using ClinicBooking.Application.Features.DanhMuc.Commands.CapNhatPhong;
using ClinicBooking.Application.Features.DanhMuc.Commands.TaoChuyenKhoa;
using ClinicBooking.Application.Features.DanhMuc.Commands.TaoDinhNghiaCa;
using ClinicBooking.Application.Features.DanhMuc.Commands.TaoDichVu;
using ClinicBooking.Application.Features.DanhMuc.Commands.TaoPhong;
using ClinicBooking.Application.Features.DanhMuc.Commands.XoaChuyenKhoa;
using ClinicBooking.Application.Features.DanhMuc.Commands.XoaDinhNghiaCa;
using ClinicBooking.Application.Features.DanhMuc.Commands.XoaDichVu;
using ClinicBooking.Application.Features.DanhMuc.Commands.XoaPhong;
using ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachChuyenKhoa;
using ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachChuyenKhoaCongKhai;
using ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachDinhNghiaCa;
using ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachDichVu;
using ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachDichVuCongKhai;
using ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachPhong;
using ClinicBooking.Application.Features.DanhMuc.Queries.LayChuyenKhoaById;
using ClinicBooking.Application.Features.DanhMuc.Queries.LayDinhNghiaCaById;
using ClinicBooking.Application.Features.DanhMuc.Queries.LayDichVuById;
using ClinicBooking.Application.Features.DanhMuc.Queries.LayPhongById;
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

    [HttpGet("chuyen-khoa/cong-khai")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<ChuyenKhoaCongKhaiDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ChuyenKhoaCongKhaiDto>>> DanhSachChuyenKhoaCongKhai(
        [FromQuery] int soTrang = 1,
        [FromQuery] int kichThuocTrang = 20,
        [FromQuery] string? tuKhoa = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new DanhSachChuyenKhoaCongKhaiQuery(soTrang, kichThuocTrang, tuKhoa),
            cancellationToken);

        return Ok(result.Select(x => x.TuDto()).ToList());
    }

    [HttpGet("dich-vu/cong-khai")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IReadOnlyList<DichVuCongKhaiDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<DichVuCongKhaiDto>>> DanhSachDichVuCongKhai(
        [FromQuery] int soTrang = 1,
        [FromQuery] int kichThuocTrang = 20,
        [FromQuery] int? idChuyenKhoa = null,
        [FromQuery] string? tuKhoa = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new DanhSachDichVuCongKhaiQuery(soTrang, kichThuocTrang, idChuyenKhoa, tuKhoa),
            cancellationToken);

        return Ok(result.Select(x => x.TuDto()).ToList());
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

    [HttpPost("phong")]
    [Authorize(Roles = VaiTroConstants.Admin)]
    [ProducesResponseType(typeof(TaoPhongResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<TaoPhongResponse>> TaoPhong(
        [FromBody] TaoPhongRequest request,
        CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(
            new TaoPhongCommand(
                request.MaPhong,
                request.TenPhong,
                request.SucChua,
                request.TrangBi,
                request.TrangThai),
            cancellationToken);

        return StatusCode(StatusCodes.Status201Created, new TaoPhongResponse(id));
    }

    [HttpGet("phong")]
    [Authorize(Roles = $"{VaiTroConstants.Admin},{VaiTroConstants.LeTan},{VaiTroConstants.BacSi}")]
    [ProducesResponseType(typeof(IReadOnlyList<PhongDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PhongDto>>> DanhSachPhong(
        [FromQuery] int soTrang = 1,
        [FromQuery] int kichThuocTrang = 20,
        [FromQuery] bool? trangThai = null,
        [FromQuery] string? tuKhoa = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new DanhSachPhongQuery(soTrang, kichThuocTrang, trangThai, tuKhoa),
            cancellationToken);

        return Ok(result.Select(x => x.TuDto()).ToList());
    }

    [HttpGet("phong/{idPhong:int}")]
    [Authorize(Roles = $"{VaiTroConstants.Admin},{VaiTroConstants.LeTan},{VaiTroConstants.BacSi}")]
    [ProducesResponseType(typeof(PhongDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PhongDto>> LayPhongById(
        int idPhong,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new LayPhongByIdQuery(idPhong), cancellationToken);
        return Ok(result.TuDto());
    }

    [HttpPut("phong/{idPhong:int}")]
    [Authorize(Roles = VaiTroConstants.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> CapNhatPhong(
        int idPhong,
        [FromBody] CapNhatPhongRequest request,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new CapNhatPhongCommand(
                idPhong,
                request.MaPhong,
                request.TenPhong,
                request.SucChua,
                request.TrangBi,
                request.TrangThai),
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("phong/{idPhong:int}")]
    [Authorize(Roles = VaiTroConstants.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> XoaPhong(
        int idPhong,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(new XoaPhongCommand(idPhong), cancellationToken);
        return NoContent();
    }

    [HttpPost("dinh-nghia-ca")]
    [Authorize(Roles = VaiTroConstants.Admin)]
    [ProducesResponseType(typeof(TaoDinhNghiaCaResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<TaoDinhNghiaCaResponse>> TaoDinhNghiaCa(
        [FromBody] TaoDinhNghiaCaRequest request,
        CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(
            new TaoDinhNghiaCaCommand(
                request.TenCa,
                request.GioBatDauMacDinh,
                request.GioKetThucMacDinh,
                request.MoTa,
                request.TrangThai),
            cancellationToken);

        return StatusCode(StatusCodes.Status201Created, new TaoDinhNghiaCaResponse(id));
    }

    [HttpGet("dinh-nghia-ca")]
    [Authorize(Roles = $"{VaiTroConstants.Admin},{VaiTroConstants.LeTan},{VaiTroConstants.BacSi}")]
    [ProducesResponseType(typeof(IReadOnlyList<DinhNghiaCaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<DinhNghiaCaDto>>> DanhSachDinhNghiaCa(
        [FromQuery] int soTrang = 1,
        [FromQuery] int kichThuocTrang = 20,
        [FromQuery] bool? trangThai = null,
        [FromQuery] string? tuKhoa = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new DanhSachDinhNghiaCaQuery(soTrang, kichThuocTrang, trangThai, tuKhoa),
            cancellationToken);

        return Ok(result.Select(x => x.TuDto()).ToList());
    }

    [HttpGet("dinh-nghia-ca/{idDinhNghiaCa:int}")]
    [Authorize(Roles = $"{VaiTroConstants.Admin},{VaiTroConstants.LeTan},{VaiTroConstants.BacSi}")]
    [ProducesResponseType(typeof(DinhNghiaCaDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<DinhNghiaCaDto>> LayDinhNghiaCaById(
        int idDinhNghiaCa,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new LayDinhNghiaCaByIdQuery(idDinhNghiaCa), cancellationToken);
        return Ok(result.TuDto());
    }

    [HttpPut("dinh-nghia-ca/{idDinhNghiaCa:int}")]
    [Authorize(Roles = VaiTroConstants.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> CapNhatDinhNghiaCa(
        int idDinhNghiaCa,
        [FromBody] CapNhatDinhNghiaCaRequest request,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new CapNhatDinhNghiaCaCommand(
                idDinhNghiaCa,
                request.TenCa,
                request.GioBatDauMacDinh,
                request.GioKetThucMacDinh,
                request.MoTa,
                request.TrangThai),
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("dinh-nghia-ca/{idDinhNghiaCa:int}")]
    [Authorize(Roles = VaiTroConstants.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> XoaDinhNghiaCa(
        int idDinhNghiaCa,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(new XoaDinhNghiaCaCommand(idDinhNghiaCa), cancellationToken);
        return NoContent();
    }
}
