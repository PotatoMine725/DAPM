using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Features.NghiPhep.Commands.DuyetDonNghiPhep;
using ClinicBooking.Application.Features.NghiPhep.Commands.TuChoiDonNghiPhep;
using ClinicBooking.Application.Features.NghiPhep.Dtos;
using ClinicBooking.Application.Features.NghiPhep.Queries.DanhSachDonNghiPhep;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicBooking.Web.Pages.LeTan;

[Authorize(Roles = "le_tan,admin")]
public class NghiPhepModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public NghiPhepModel(IMediator mediator, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    public IReadOnlyList<DonNghiPhepResponse> DanhSach { get; private set; } = [];
    public TrangThaiDuyetDon? TrangThaiLoc { get; private set; }

    [BindProperty] public string? LyDoTuChoi { get; set; }

    public async Task OnGetAsync(TrangThaiDuyetDon? trangThai)
    {
        TrangThaiLoc = trangThai;
        DanhSach = await _mediator.Send(new DanhSachDonNghiPhepQuery(trangThai));
    }

    public async Task<IActionResult> OnPostDuyetAsync(int idDonNghiPhep, TrangThaiDuyetDon? trangThai)
    {
        try
        {
            var idNguoiDuyet = _currentUser.IdTaiKhoan ?? 0;
            await _mediator.Send(new DuyetDonNghiPhepCommand(idDonNghiPhep, true, null, idNguoiDuyet));
            TempData["SuccessMessage"] = "Đã duyệt đơn nghỉ phép.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToPage(new { trangThai });
    }

    public async Task<IActionResult> OnPostTuChoiAsync(int idDonNghiPhep, TrangThaiDuyetDon? trangThai)
    {
        try
        {
            var idNguoiDuyet = _currentUser.IdTaiKhoan ?? 0;
            await _mediator.Send(new TuChoiDonNghiPhepCommand(idDonNghiPhep, idNguoiDuyet, LyDoTuChoi ?? string.Empty));
            TempData["SuccessMessage"] = "Đã từ chối đơn nghỉ phép.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToPage(new { trangThai });
    }
}
