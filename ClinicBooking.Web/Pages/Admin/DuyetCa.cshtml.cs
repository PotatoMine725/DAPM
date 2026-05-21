using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Constants;
using ClinicBooking.Application.Features.Scheduling.Commands.DuyetCaLamViec;
using ClinicBooking.Application.Features.Scheduling.Commands.DuyetNhieuCaLamViec;
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
    [BindProperty] public List<int> DanhSachIdCaDuyet { get; set; } = [];

    // Filter properties
    [BindProperty(SupportsGet = true)] public int? IdBacSiLoc { get; set; }
    [BindProperty(SupportsGet = true)] public NguonTaoCa? NguonTaoCaLoc { get; set; }
    [BindProperty(SupportsGet = true)] public int? IdChuyenKhoaLoc { get; set; }
    [BindProperty(SupportsGet = true)] public DateOnly? TuNgayLoc { get; set; }
    [BindProperty(SupportsGet = true)] public DateOnly? DenNgayLoc { get; set; }

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

    public async Task<IActionResult> OnPostDuyetNhieuAsync()
    {
        if (DanhSachIdCaDuyet == null || !DanhSachIdCaDuyet.Any())
        {
            TempData["ErrorMessage"] = "Vui lòng chọn ít nhất một ca để duyệt.";
            return RedirectToPage();
        }

        try
        {
            var idAdmin = _currentUser.IdTaiKhoan ?? 0;
            var result = await _mediator.Send(new DuyetNhieuCaLamViecCommand(DanhSachIdCaDuyet, idAdmin));

            if (result.SoThanhCong > 0)
            {
                TempData["SuccessMessage"] = $"Đã duyệt thành công {result.SoThanhCong} ca.";
            }

            if (result.SoThatBai > 0)
            {
                var chiTietLoi = string.Join(", ", result.DanhSachLoi.Select(x => $"Ca #{x.IdCaLamViec}: {x.LyDo}"));
                TempData["ErrorMessage"] = $"{result.SoThatBai} ca thất bại. Chi tiết: {chiTietLoi}";
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToPage();
    }

    private async Task TaiDuLieuAsync()
    {
        var tuNgay = TuNgayLoc ?? new DateOnly(DateTime.Today.Year, DateTime.Today.Month, 1);
        var denNgay = DenNgayLoc ?? tuNgay.AddMonths(1).AddDays(-1);

        ThongKe = await _mediator.Send(new ThongKeDuyetCaQuery(tuNgay, denNgay));

        DanhSachChoDuyet = await _mediator.Send(new DanhSachCaLamViecChoDuyetQuery(
            SoTrang: 1,
            KichThuocTrang: 100,
            TrangThaiDuyet: TrangThaiDuyetCa.ChoDuyet,
            NguonTaoCa: NguonTaoCaLoc ?? NguonTaoCa.BacSiDangKy,
            IdBacSi: IdBacSiLoc,
            IdChuyenKhoa: IdChuyenKhoaLoc,
            TuNgay: TuNgayLoc,
            DenNgay: DenNgayLoc));

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
