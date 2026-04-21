using ClinicBooking.Application.Features.HangCho.Commands.GoiBenhNhanKeTiep;
using ClinicBooking.Application.Features.HangCho.Commands.HoanThanhLuotKham;
using ClinicBooking.Application.Features.HangCho.Dtos;
using ClinicBooking.Application.Features.HangCho.Queries.XemHangChoTheoCa;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicBooking.Web.Pages.BacSi;

[Authorize(Roles = "bac_si")]
public class HangChoModel : PageModel
{
    private readonly IMediator _mediator;

    public HangChoModel(IMediator mediator) => _mediator = mediator;

    public IReadOnlyList<HangChoResponse> HangCho { get; private set; } = [];
    public int? IdCaLamViec { get; private set; }

    public int SoChoKham   => HangCho.Count(x => x.TrangThai == TrangThaiHangCho.ChoKham);
    public int SoDangKham  => HangCho.Count(x => x.TrangThai == TrangThaiHangCho.DangKham);
    public int SoHoanThanh => HangCho.Count(x => x.TrangThai == TrangThaiHangCho.HoanThanh);

    public async Task OnGetAsync(int? idCaLamViec)
    {
        IdCaLamViec = idCaLamViec;
        if (idCaLamViec.HasValue)
        {
            HangCho = await _mediator.Send(new XemHangChoTheoCaQuery(idCaLamViec.Value));
        }
    }

    public async Task<IActionResult> OnPostGoiKeTiepAsync(int idCaLamViec)
    {
        try
        {
            await _mediator.Send(new GoiBenhNhanKeTiepCommand(idCaLamViec));
            TempData["SuccessMessage"] = "Đã gọi bệnh nhân tiếp theo.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToPage(new { idCaLamViec });
    }

    public async Task<IActionResult> OnPostHoanThanhAsync(int idHangCho, int idCaLamViec)
    {
        try
        {
            await _mediator.Send(new HoanThanhLuotKhamCommand(idHangCho));
            TempData["SuccessMessage"] = "Đã hoàn thành lượt khám.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToPage(new { idCaLamViec });
    }
}
