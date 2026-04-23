using ClinicBooking.Application.Features.HangCho.Dtos;
using ClinicBooking.Application.Features.HangCho.Queries.ThuTuCuaToi;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicBooking.Web.Pages.BenhNhan;

[Authorize(Roles = "benh_nhan")]
public class ThuTuHangChoModel : PageModel
{
    private readonly IMediator _mediator;

    public ThuTuHangChoModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public ThuTuHangChoResponse KetQua { get; private set; } = new(0, 0, 0, 0, false, null);
    public string? ErrorMessage { get; private set; }

    public async Task OnGetAsync(int? idCaLamViec)
    {
        if (!idCaLamViec.HasValue || idCaLamViec <= 0)
        {
            ErrorMessage = "Vui lòng chọn một ca làm việc để xem thứ tự hàng chờ.";
            return;
        }

        try
        {
            KetQua = await _mediator.Send(new ThuTuCuaToiQuery(idCaLamViec.Value));
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }
}
