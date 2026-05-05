using ClinicBooking.Application.Features.LichHen.Commands.HuyLichHen;
using ClinicBooking.Application.Features.LichHen.Dtos;
using ClinicBooking.Application.Features.LichHen.Queries.DanhSachLichHenCuaToi;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicBooking.Web.Pages.BenhNhan;

[Authorize(Roles = "benh_nhan")]
public class DanhSachLichHenModel : PageModel
{
    private readonly IMediator _mediator;
    private const int KichThuocTrang = 5;

    public DanhSachLichHenModel(IMediator mediator) => _mediator = mediator;

    public DanhSachLichHenResponse DanhSach { get; private set; } =
        new([], 0, 1, KichThuocTrang);

    public TrangThaiLichHen? TrangThaiFilter { get; private set; }

    public int TongSoTrang =>
        DanhSach.TongSo == 0 ? 1 : (int)Math.Ceiling((double)DanhSach.TongSo / KichThuocTrang);

    public async Task OnGetAsync(TrangThaiLichHen? trangThai, int soTrang = 1)
    {
        TrangThaiFilter = trangThai;
        var query = new DanhSachLichHenCuaToiQuery(trangThai, soTrang, KichThuocTrang);
        DanhSach = await _mediator.Send(query);
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
        return RedirectToPage();
    }
}
