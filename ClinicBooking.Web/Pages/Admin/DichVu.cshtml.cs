using ClinicBooking.Application.Common.Constants;
using ClinicBooking.Application.Features.DanhMuc.Commands.CapNhatDichVu;
using ClinicBooking.Application.Features.DanhMuc.Commands.TaoDichVu;
using ClinicBooking.Application.Features.DanhMuc.Commands.XoaDichVu;
using ClinicBooking.Application.Features.DanhMuc.Dtos;
using ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachChuyenKhoa;
using ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachDichVu;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicBooking.Web.Pages.Admin;

[Authorize(Roles = VaiTroConstants.Admin)]
public class DichVuModel : PageModel
{
    private readonly IMediator _mediator;

    public DichVuModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public IReadOnlyList<DichVuResponse> DanhSach { get; private set; } = [];
    public IReadOnlyList<ChuyenKhoaResponse> DanhSachChuyenKhoa { get; private set; } = [];

    public int? IdChuyenKhoaLoc { get; private set; }
    public bool? HienThiLoc { get; private set; }
    public string? TuKhoa { get; private set; }

    [BindProperty] public int IdDichVuSua { get; set; }
    [BindProperty] public int IdChuyenKhoaInput { get; set; }
    [BindProperty] public string TenDichVu { get; set; } = string.Empty;
    [BindProperty] public string? MoTa { get; set; }
    [BindProperty] public int? ThoiGianUocTinh { get; set; }
    [BindProperty] public bool HienThi { get; set; } = true;

    [BindProperty] public int IdDichVuXoa { get; set; }

    public async Task OnGetAsync(int? idChuyenKhoa = null, bool? hienThi = null, string? tuKhoa = null)
    {
        IdChuyenKhoaLoc = idChuyenKhoa;
        HienThiLoc = hienThi;
        TuKhoa = tuKhoa;
        DanhSachChuyenKhoa = await _mediator.Send(new DanhSachChuyenKhoaQuery(1, 200, true, null));
        DanhSach = await _mediator.Send(new DanhSachDichVuQuery(1, 200, idChuyenKhoa, hienThi, tuKhoa));
    }

    public async Task<IActionResult> OnPostTaoAsync()
    {
        try
        {
            await _mediator.Send(new TaoDichVuCommand(IdChuyenKhoaInput, TenDichVu.Trim(), MoTa, ThoiGianUocTinh, HienThi));
            TempData["SuccessMessage"] = $"Đã tạo dịch vụ {TenDichVu}.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostCapNhatAsync()
    {
        try
        {
            await _mediator.Send(new CapNhatDichVuCommand(IdDichVuSua, IdChuyenKhoaInput, TenDichVu.Trim(), MoTa, ThoiGianUocTinh, HienThi));
            TempData["SuccessMessage"] = "Đã cập nhật dịch vụ.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostXoaAsync()
    {
        try
        {
            await _mediator.Send(new XoaDichVuCommand(IdDichVuXoa));
            TempData["SuccessMessage"] = "Đã xoá dịch vụ.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToPage();
    }
}
