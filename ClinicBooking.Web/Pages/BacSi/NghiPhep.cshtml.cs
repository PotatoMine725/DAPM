using ClinicBooking.Application.Features.NghiPhep.Commands.NopDonNghiPhep;
using ClinicBooking.Application.Features.Scheduling.Dtos;
using ClinicBooking.Application.Features.Scheduling.Queries.DanhSachCaLamViecCongKhai;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicBooking.Web.Pages.BacSi;

[Authorize(Roles = "bac_si")]
public class NghiPhepModel : PageModel
{
    private readonly IMediator _mediator;

    public NghiPhepModel(IMediator mediator) => _mediator = mediator;

    public IReadOnlyList<CaLamViecPublicResponse> DanhSachCa { get; private set; } = [];

    [BindProperty] public int IdCaLamViec { get; set; }
    [BindProperty] public string LoaiNghiPhep { get; set; } = "TamNghi";
    [BindProperty] public string LyDo { get; set; } = string.Empty;
    [BindProperty] public DateOnly TuNgay { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow.Date);

    public async Task OnGetAsync(DateOnly? tuNgay = null)
    {
        TuNgay = tuNgay ?? DateOnly.FromDateTime(DateTime.UtcNow.Date);
        DanhSachCa = await _mediator.Send(new DanhSachCaLamViecCongKhaiQuery(
            SoTrang: 1,
            KichThuocTrang: 50,
            TuNgay: TuNgay,
            DenNgay: TuNgay.AddDays(30),
            ConTrong: null));
    }

    public async Task<IActionResult> OnPostAsync(DateOnly? tuNgay = null)
    {
        TuNgay = tuNgay ?? DateOnly.FromDateTime(DateTime.UtcNow.Date);

        try
        {
            var idBacSi = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            await _mediator.Send(new NopDonNghiPhepCommand(idBacSi, IdCaLamViec, LoaiNghiPhep, LyDo));
            TempData["SuccessMessage"] = "Đã gửi đơn nghỉ phép.";
            return RedirectToPage("/BacSi/NghiPhep", new { tuNgay = TuNgay });
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            DanhSachCa = await _mediator.Send(new DanhSachCaLamViecCongKhaiQuery(
                SoTrang: 1,
                KichThuocTrang: 50,
                TuNgay: TuNgay,
                DenNgay: TuNgay.AddDays(30),
                ConTrong: null));
            return Page();
        }
    }
}
