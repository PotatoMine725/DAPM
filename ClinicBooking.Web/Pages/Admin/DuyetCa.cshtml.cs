using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Constants;
using ClinicBooking.Application.Features.Scheduling.Commands.DuyetCaLamViec;
using ClinicBooking.Application.Features.Scheduling.Dtos;
using ClinicBooking.Application.Features.Scheduling.Queries.DanhSachCaLamViecChoDuyet;
using ClinicBooking.Application.Features.Scheduling.Queries.ThongKeDuyetCa;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicBooking.Web.Pages.Admin;

[Authorize(Roles = VaiTroConstants.Admin)]
public class DuyetCaModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public DuyetCaModel(IMediator mediator, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    public ThongKeDuyetCaResponse ThongKe { get; private set; } = new(0, 0, 0, 0);
    public IReadOnlyList<CaLamViecAdminResponse> DanhSachChoDuyet { get; private set; } = [];
    public IReadOnlyList<CaLamViecAdminResponse> DanhSachDaXuLy { get; private set; } = [];

    [BindProperty] public int IdCaLamViecChon { get; set; }
    [BindProperty] public string? LyDoTuChoi { get; set; }

    public async Task OnGetAsync()
    {
        await TaiDuLieuAsync();
    }

    public async Task<IActionResult> OnPostDuyetAsync()
    {
        try
        {
            var idAdmin = _currentUser.IdTaiKhoan ?? 0;
            await _mediator.Send(new DuyetCaLamViecCommand(IdCaLamViecChon, true, null, idAdmin));
            TempData["SuccessMessage"] = "Đã duyệt ca làm việc.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostTuChoiAsync()
    {
        if (string.IsNullOrWhiteSpace(LyDoTuChoi))
        {
            TempData["ErrorMessage"] = "Vui lòng nhập lý do từ chối.";
            return RedirectToPage();
        }
        try
        {
            var idAdmin = _currentUser.IdTaiKhoan ?? 0;
            await _mediator.Send(new DuyetCaLamViecCommand(IdCaLamViecChon, false, LyDoTuChoi.Trim(), idAdmin));
            TempData["SuccessMessage"] = "Đã từ chối ca làm việc.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToPage();
    }

    private async Task TaiDuLieuAsync()
    {
        var dauThang = new DateOnly(DateTime.Today.Year, DateTime.Today.Month, 1);
        var cuoiThang = dauThang.AddMonths(1).AddDays(-1);

        ThongKe = await _mediator.Send(new ThongKeDuyetCaQuery(dauThang, cuoiThang));

        DanhSachChoDuyet = await _mediator.Send(new DanhSachCaLamViecChoDuyetQuery(
            SoTrang: 1,
            KichThuocTrang: 100,
            TrangThaiDuyet: TrangThaiDuyetCa.ChoDuyet,
            NguonTaoCa: NguonTaoCa.BacSiDangKy));

        // 20 ca xu ly gan day (DaDuyet hoac DaHuy)
        var daDuyet = await _mediator.Send(new DanhSachCaLamViecChoDuyetQuery(
            SoTrang: 1,
            KichThuocTrang: 10,
            TrangThaiDuyet: TrangThaiDuyetCa.DaDuyet,
            NguonTaoCa: NguonTaoCa.BacSiDangKy));
        var daHuy = await _mediator.Send(new DanhSachCaLamViecChoDuyetQuery(
            SoTrang: 1,
            KichThuocTrang: 10,
            TrangThaiDuyet: TrangThaiDuyetCa.DaHuy,
            NguonTaoCa: NguonTaoCa.BacSiDangKy));
        DanhSachDaXuLy = daDuyet
            .Concat(daHuy)
            .OrderByDescending(x => x.NgayDuyet ?? x.NgayTao)
            .Take(20)
            .ToList();
    }
}
