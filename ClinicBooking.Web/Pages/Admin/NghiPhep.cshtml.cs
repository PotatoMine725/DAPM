using ClinicBooking.Application.Common.Constants;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.NghiPhep.Commands.DuyetDonNghiPhep;
using ClinicBooking.Application.Features.NghiPhep.Queries.DanhSachDonNghiPhepChoDuyet;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicBooking.Web.Pages.Admin;

[Authorize(Roles = VaiTroConstants.Admin)]
public class NghiPhepModel : PageModel
{
    private readonly IMediator _mediator;

    public NghiPhepModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public IReadOnlyList<ClinicBooking.Application.Features.NghiPhep.Dtos.DonNghiPhepResponse> DanhSach { get; private set; } = [];
    public int SoDonChoDuyet => DanhSach.Count;
    public int SoNgayNghiThangNay { get; private set; }

    public async Task OnGetAsync()
    {
        DanhSach = await _mediator.Send(new DanhSachDonNghiPhepChoDuyetQuery());
        SoNgayNghiThangNay = DanhSach.Count(x => x.NgayGuiDon.Month == DateTime.UtcNow.Month && x.NgayGuiDon.Year == DateTime.UtcNow.Year);
    }

    public async Task<IActionResult> OnPostApproveAsync(int idDonNghiPhep)
    {
        try
        {
            var idNguoiDuyet = int.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var id) ? id : 0;
            await _mediator.Send(new DuyetDonNghiPhepCommand(idDonNghiPhep, true, null, idNguoiDuyet));
            TempData["SuccessMessage"] = "Đã duyệt đơn nghỉ phép.";
        }
        catch (ConflictException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRejectAsync(int idDonNghiPhep, string lyDoTuChoi)
    {
        try
        {
            var idNguoiDuyet = int.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out var id) ? id : 0;
            await _mediator.Send(new DuyetDonNghiPhepCommand(idDonNghiPhep, false, lyDoTuChoi, idNguoiDuyet));
            TempData["SuccessMessage"] = "Đã từ chối đơn nghỉ phép.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToPage();
    }
}