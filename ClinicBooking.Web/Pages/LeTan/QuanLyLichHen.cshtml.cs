using ClinicBooking.Application.Common;
using ClinicBooking.Application.Features.LichHen.Commands.CheckInLichHen;
using ClinicBooking.Application.Features.LichHen.Commands.HuyLichHen;
using ClinicBooking.Application.Features.LichHen.Commands.XacNhanLichHen;
using ClinicBooking.Application.Features.LichHen.Dtos;
using ClinicBooking.Application.Features.LichHen.Queries.DanhSachTatCaLichHen;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicBooking.Web.Pages.LeTan;

[Authorize(Roles = "le_tan,admin")]
public class QuanLyLichHenModel : PageModel
{
    private readonly IMediator _mediator;
    private const int SoTrenMoiTrang = 10;

    public QuanLyLichHenModel(IMediator mediator) => _mediator = mediator;

    public PhanTrangKetQua<LichHenTomTatResponse> KetQua { get; private set; } = null!;
    public DateOnly? NgayLoc { get; private set; }
    public TrangThaiLichHen? TrangThaiLoc { get; private set; }
    public int TrangHienTai { get; private set; }

    public async Task OnGetAsync(DateOnly? ngay, TrangThaiLichHen? trangThai, int trang = 1)
    {
        NgayLoc = ngay;
        TrangThaiLoc = trangThai;
        TrangHienTai = trang;

        KetQua = await _mediator.Send(
            new DanhSachTatCaLichHenQuery(ngay, trangThai, trang, SoTrenMoiTrang));
    }

    public async Task<IActionResult> OnPostXacNhanAsync(int idLichHen)
    {
        try
        {
            await _mediator.Send(new XacNhanLichHenCommand(idLichHen));
            TempData["SuccessMessage"] = "Đã xác nhận lịch hẹn.";
        }
        catch (Exception ex) { TempData["ErrorMessage"] = ex.Message; }
        return RedirectToPage(new { ngay = Request.Query["ngay"], trangThai = Request.Query["trangThai"], trang = Request.Query["trang"] });
    }

    public async Task<IActionResult> OnPostCheckInAsync(int idLichHen)
    {
        try
        {
            await _mediator.Send(new CheckInLichHenCommand(idLichHen));
            TempData["SuccessMessage"] = "Check-in thành công.";
        }
        catch (Exception ex) { TempData["ErrorMessage"] = ex.Message; }
        return RedirectToPage(new { ngay = Request.Query["ngay"], trangThai = Request.Query["trangThai"], trang = Request.Query["trang"] });
    }

    public async Task<IActionResult> OnPostHuyAsync(int idLichHen, string lyDo)
    {
        try
        {
            await _mediator.Send(new HuyLichHenCommand(idLichHen, lyDo));
            TempData["SuccessMessage"] = "Đã huỷ lịch hẹn.";
        }
        catch (Exception ex) { TempData["ErrorMessage"] = ex.Message; }
        return RedirectToPage(new { ngay = Request.Query["ngay"], trangThai = Request.Query["trangThai"], trang = Request.Query["trang"] });
    }
}
