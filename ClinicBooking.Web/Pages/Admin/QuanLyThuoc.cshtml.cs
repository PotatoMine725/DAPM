using ClinicBooking.Application.Common.Constants;
using ClinicBooking.Application.Features.Thuoc.Commands.CapNhatThuoc;
using ClinicBooking.Application.Features.Thuoc.Commands.TaoThuoc;
using ClinicBooking.Application.Features.Thuoc.Commands.XoaThuoc;
using ClinicBooking.Application.Features.Thuoc.Dtos;
using ClinicBooking.Application.Features.Thuoc.Queries.DanhSachThuoc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicBooking.Web.Pages.Admin;

[Authorize(Roles = VaiTroConstants.Admin)]
public class QuanLyThuocModel : PageModel
{
    private readonly IMediator _mediator;
    private const int KichThuocTrangMacDinh = 20;

    public QuanLyThuocModel(IMediator mediator) => _mediator = mediator;

    public IReadOnlyList<ThuocResponse> DanhSachThuoc { get; private set; } = [];

    [BindProperty(SupportsGet = true)]
    public string? TuKhoa { get; set; }

    [BindProperty(SupportsGet = true)]
    public int SoTrang { get; set; } = 1;

    [BindProperty]
    public ThuocFormInput Thuoc { get; set; } = new();

    public async Task OnGetAsync()
    {
        await TaiDanhSachAsync();
    }

    public async Task<IActionResult> OnPostTaoAsync()
    {
        try
        {
            await _mediator.Send(new TaoThuocCommand(
                Thuoc.TenThuoc,
                Thuoc.HoatChat,
                Thuoc.DonVi,
                Thuoc.GhiChu));
            TempData["SuccessMessage"] = "Da tao thuoc moi.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToPage(new { tuKhoa = TuKhoa, soTrang = SoTrang });
    }

    public async Task<IActionResult> OnPostCapNhatAsync()
    {
        try
        {
            await _mediator.Send(new CapNhatThuocCommand(
                Thuoc.IdThuoc,
                Thuoc.TenThuoc,
                Thuoc.HoatChat,
                Thuoc.DonVi,
                Thuoc.GhiChu));
            TempData["SuccessMessage"] = "Da cap nhat thuoc.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToPage(new { tuKhoa = TuKhoa, soTrang = SoTrang });
    }

    public async Task<IActionResult> OnPostXoaAsync(int idThuoc)
    {
        try
        {
            await _mediator.Send(new XoaThuocCommand(idThuoc));
            TempData["SuccessMessage"] = "Da xoa thuoc.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToPage(new { tuKhoa = TuKhoa, soTrang = SoTrang });
    }

    private async Task TaiDanhSachAsync()
    {
        DanhSachThuoc = await _mediator.Send(new DanhSachThuocQuery(
            Math.Max(1, SoTrang),
            KichThuocTrangMacDinh,
            TuKhoa));
    }

    public sealed class ThuocFormInput
    {
        public int IdThuoc { get; set; }
        public string TenThuoc { get; set; } = string.Empty;
        public string? HoatChat { get; set; }
        public string? DonVi { get; set; }
        public string? GhiChu { get; set; }
    }
}
