using ClinicBooking.Application.Features.ThongBao.Commands.DanhDauDaDocThongBao;
using ClinicBooking.Application.Features.ThongBao.Queries.DanhSachThongBaoCuaToi;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicBooking.Web.Pages.BenhNhan;

[Authorize(Roles = "benh_nhan")]
public class ThongBaoModel : PageModel
{
    private readonly IMediator _mediator;
    private const int KichThuocTrang = 15;

    public ThongBaoModel(IMediator mediator) => _mediator = mediator;

    public DanhSachThongBaoResponse DanhSach { get; private set; } =
        new([], 0, 1, KichThuocTrang, 0);

    public bool? ChiChuaDocFilter { get; private set; }

    public int TongSoTrang =>
        DanhSach.TongSo == 0 ? 1 : (int)Math.Ceiling((double)DanhSach.TongSo / KichThuocTrang);

    public async Task OnGetAsync(bool? chiChuaDoc = null, int soTrang = 1)
    {
        ChiChuaDocFilter = chiChuaDoc;
        DanhSach = await _mediator.Send(
            new DanhSachThongBaoCuaToiQuery(chiChuaDoc, soTrang, KichThuocTrang));
    }

    // Danh dau 1 thong bao la da doc
    public async Task<IActionResult> OnPostDocAsync(int idThongBao)
    {
        try
        {
            await _mediator.Send(new DanhDauDaDocThongBaoCommand(idThongBao));
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToPage();
    }

    // Danh dau tat ca la da doc
    public async Task<IActionResult> OnPostDocTatCaAsync()
    {
        try
        {
            await _mediator.Send(new DanhDauDaDocThongBaoCommand(null));
            TempData["SuccessMessage"] = "Đã đánh dấu tất cả thông báo là đã đọc.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToPage();
    }
}
