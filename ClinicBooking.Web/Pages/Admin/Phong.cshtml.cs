using ClinicBooking.Application.Common.Constants;
using ClinicBooking.Application.Features.DanhMuc.Commands.CapNhatPhong;
using ClinicBooking.Application.Features.DanhMuc.Commands.TaoPhong;
using ClinicBooking.Application.Features.DanhMuc.Commands.XoaPhong;
using ClinicBooking.Application.Features.DanhMuc.Dtos;
using ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachPhong;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicBooking.Web.Pages.Admin;

[Authorize(Roles = VaiTroConstants.Admin)]
public class PhongModel : PageModel
{
    private readonly IMediator _mediator;

    public PhongModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public IReadOnlyList<PhongResponse> DanhSach { get; private set; } = [];
    public bool? TrangThaiLoc { get; private set; }
    public string? TuKhoa { get; private set; }

    [BindProperty] public int IdPhongSua { get; set; }
    [BindProperty] public string MaPhong { get; set; } = string.Empty;
    [BindProperty] public string TenPhong { get; set; } = string.Empty;
    [BindProperty] public int? SucChua { get; set; }
    [BindProperty] public string? TrangBi { get; set; }
    [BindProperty] public bool TrangThai { get; set; } = true;

    [BindProperty] public int IdPhongXoa { get; set; }

    public async Task OnGetAsync(bool? trangThai = null, string? tuKhoa = null)
    {
        TrangThaiLoc = trangThai;
        TuKhoa = tuKhoa;
        DanhSach = await _mediator.Send(new DanhSachPhongQuery(1, 200, trangThai, tuKhoa));
    }

    public async Task<IActionResult> OnPostTaoAsync()
    {
        try
        {
            await _mediator.Send(new TaoPhongCommand(MaPhong.Trim(), TenPhong.Trim(), SucChua, TrangBi, TrangThai));
            TempData["SuccessMessage"] = $"Đã tạo phòng {MaPhong}.";
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
            await _mediator.Send(new CapNhatPhongCommand(IdPhongSua, MaPhong.Trim(), TenPhong.Trim(), SucChua, TrangBi, TrangThai));
            TempData["SuccessMessage"] = $"Đã cập nhật phòng {MaPhong}.";
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
            await _mediator.Send(new XoaPhongCommand(IdPhongXoa));
            TempData["SuccessMessage"] = "Đã xoá phòng.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToPage();
    }
}
