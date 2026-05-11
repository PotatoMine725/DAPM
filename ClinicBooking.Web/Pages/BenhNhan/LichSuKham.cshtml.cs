using ClinicBooking.Application.Features.HoSoKham.Dtos;
using ClinicBooking.Application.Features.HoSoKham.Queries.LichSuKhamCuaToi;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicBooking.Web.Pages.BenhNhan;

[Authorize(Roles = "benh_nhan")]
public class LichSuKhamModel : PageModel
{
    private readonly IMediator _mediator;
    private const int KichThuocTrangMacDinh = 10;

    public LichSuKhamModel(IMediator mediator) => _mediator = mediator;

    public IReadOnlyList<HoSoKhamTomTatResponse> DanhSachHoSo { get; private set; } = [];

    [BindProperty(SupportsGet = true)]
    public int SoTrang { get; set; } = 1;

    public async Task OnGetAsync()
    {
        try
        {
            DanhSachHoSo = await _mediator.Send(new LichSuKhamCuaToiQuery(
                Math.Max(1, SoTrang),
                KichThuocTrangMacDinh));
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            DanhSachHoSo = [];
        }
    }
}
