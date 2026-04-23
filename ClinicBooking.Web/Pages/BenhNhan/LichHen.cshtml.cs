using ClinicBooking.Application.Features.LichHen.Commands.HuyLichHen;
using ClinicBooking.Application.Features.LichHen.Dtos;
using ClinicBooking.Application.Features.LichHen.Queries.XemLichHen;
using ClinicBooking.Domain.Enums;
using ClinicBooking.Web.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicBooking.Web.Pages.BenhNhan;

[Authorize(Roles = "benh_nhan")]
public class LichHenModel : PageModel
{
    private readonly IMediator _mediator;

    public LichHenModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public LichHenResponse LichHen { get; private set; } = default!;
    public IReadOnlyList<LichSuLichHenViewModel> LichSu { get; private set; } = [];

    public async Task OnGetAsync(int idLichHen)
    {
        LichHen = await _mediator.Send(new XemLichHenQuery(idLichHen));
        LichSu = TaoLichSuGioiThieu(LichHen);
    }

    public async Task<IActionResult> OnPostHuyAsync(int idLichHen, string? lyDo)
    {
        try
        {
            await _mediator.Send(new HuyLichHenCommand(idLichHen, lyDo ?? string.Empty));
            TempData["SuccessMessage"] = "Đã huỷ lịch hẹn thành công.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToPage(new { idLichHen });
    }

    private static IReadOnlyList<LichSuLichHenViewModel> TaoLichSuGioiThieu(LichHenResponse lichHen)
    {
        var items = new List<LichSuLichHenViewModel>
        {
            new($"Đặt lịch {BadgeHelper.TrangThaiText(lichHen.TrangThai)}", lichHen.NgayTao)
        };

        return items;
    }
}

public sealed record LichSuLichHenViewModel(string HanhDong, DateTime NgayTao);
