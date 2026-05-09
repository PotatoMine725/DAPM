using ClinicBooking.Application.Features.BacSi.Queries.LayLichLamViecCuaToi;
using ClinicBooking.Application.Features.Scheduling.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicBooking.Web.Pages.BacSi;

[Authorize(Roles = "bac_si")]
public class LichLamViecModel : PageModel
{
    private readonly IMediator _mediator;

    public LichLamViecModel(IMediator mediator) => _mediator = mediator;

    public IReadOnlyList<CaLamViecPublicResponse> DanhSachCa { get; private set; } = [];
    public DateOnly TuNgay { get; private set; }
    public DateOnly DenNgay { get; private set; }
    public int SoTuan { get; private set; } = 4;

    public async Task OnGetAsync(DateOnly? tuNgay = null, int soTuan = 4)
    {
        TuNgay = tuNgay ?? DateOnly.FromDateTime(DateTime.UtcNow);
        SoTuan = soTuan <= 0 ? 4 : soTuan;
        DenNgay = TuNgay.AddDays(SoTuan * 7 - 1);

        DanhSachCa = await _mediator.Send(new LayLichLamViecCuaToiQuery(TuNgay, DenNgay));
    }
}
