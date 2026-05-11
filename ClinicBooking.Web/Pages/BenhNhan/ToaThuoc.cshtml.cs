using ClinicBooking.Application.Features.ToaThuoc.Dtos;
using ClinicBooking.Application.Features.ToaThuoc.Queries.LayToaCuaToi;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicBooking.Web.Pages.BenhNhan;

[Authorize(Roles = "benh_nhan")]
public class ToaThuocModel : PageModel
{
    private readonly IMediator _mediator;
    private const int KichThuocTrangMacDinh = 10;

    public ToaThuocModel(IMediator mediator) => _mediator = mediator;

    public IReadOnlyList<ToaThuocResponse> DanhSachToa { get; private set; } = [];

    // Group prescriptions by HoSoKham (IdHoSoKham + NgayKham + MaLichHen)
    public IEnumerable<IGrouping<(int IdHoSoKham, DateTime NgayKham, string MaLichHen), ToaThuocResponse>> ToaNhomTheoHoSo =>
        DanhSachToa.GroupBy(x => (x.IdHoSoKham, x.NgayKham, x.MaLichHen));

    [BindProperty(SupportsGet = true)]
    public int SoTrang { get; set; } = 1;

    public async Task OnGetAsync()
    {
        try
        {
            DanhSachToa = await _mediator.Send(new LayToaCuaToiQuery(
                Math.Max(1, SoTrang),
                KichThuocTrangMacDinh));
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            DanhSachToa = [];
        }
    }
}
