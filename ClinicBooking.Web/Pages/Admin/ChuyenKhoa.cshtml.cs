using ClinicBooking.Application.Common.Constants;
using ClinicBooking.Application.Features.DanhMuc.Commands.CapNhatChuyenKhoa;
using ClinicBooking.Application.Features.DanhMuc.Commands.TaoChuyenKhoa;
using ClinicBooking.Application.Features.DanhMuc.Commands.XoaChuyenKhoa;
using ClinicBooking.Application.Features.DanhMuc.Dtos;
using ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachChuyenKhoa;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicBooking.Web.Pages.Admin;

[Authorize(Roles = VaiTroConstants.Admin)]
public class ChuyenKhoaModel : PageModel
{
    private readonly IMediator _mediator;

    public ChuyenKhoaModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public IReadOnlyList<ChuyenKhoaResponse> DanhSach { get; private set; } = [];
    public bool? HienThiLoc { get; private set; }
    public string? TuKhoa { get; private set; }

    [BindProperty] public int IdChuyenKhoaSua { get; set; }
    [BindProperty] public string TenChuyenKhoa { get; set; } = string.Empty;
    [BindProperty] public string? MoTa { get; set; }
    [BindProperty] public int ThoiGianSlotMacDinh { get; set; } = 20;
    [BindProperty] public TimeOnly? GioMoDatLich { get; set; }
    [BindProperty] public TimeOnly? GioDongDatLich { get; set; }
    [BindProperty] public bool HienThi { get; set; } = true;

    [BindProperty] public int IdChuyenKhoaXoa { get; set; }

    public async Task OnGetAsync(bool? hienThi = null, string? tuKhoa = null)
    {
        HienThiLoc = hienThi;
        TuKhoa = tuKhoa;
        DanhSach = await _mediator.Send(new DanhSachChuyenKhoaQuery(1, 200, hienThi, tuKhoa));
    }

    public async Task<IActionResult> OnPostTaoAsync()
    {
        try
        {
            await _mediator.Send(new TaoChuyenKhoaCommand(TenChuyenKhoa.Trim(), MoTa, ThoiGianSlotMacDinh, GioMoDatLich, GioDongDatLich, HienThi));
            TempData["SuccessMessage"] = $"Đã tạo chuyên khoa {TenChuyenKhoa}.";
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
            await _mediator.Send(new CapNhatChuyenKhoaCommand(IdChuyenKhoaSua, TenChuyenKhoa.Trim(), MoTa, ThoiGianSlotMacDinh, GioMoDatLich, GioDongDatLich, HienThi));
            TempData["SuccessMessage"] = $"Đã cập nhật chuyên khoa.";
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
            await _mediator.Send(new XoaChuyenKhoaCommand(IdChuyenKhoaXoa));
            TempData["SuccessMessage"] = "Đã xoá chuyên khoa.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToPage();
    }
}
