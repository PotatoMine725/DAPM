using ClinicBooking.Application.Features.DanhMuc.Dtos;
using ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachDichVu;
using ClinicBooking.Application.Features.LichHen.Commands.TaoLichHen;
using ClinicBooking.Application.Features.Scheduling.Dtos;
using ClinicBooking.Application.Features.Scheduling.Queries.DanhSachCaLamViecCongKhai;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicBooking.Web.Pages.BenhNhan;

[Authorize(Roles = "benh_nhan")]
public class DatLichModel : PageModel
{
    private readonly IMediator _mediator;

    public DatLichModel(IMediator mediator) => _mediator = mediator;

    // Du lieu hien thi
    public IReadOnlyList<CaLamViecPublicResponse> DanhSachCa { get; private set; } = [];
    public IReadOnlyList<DichVuResponse> DanhSachDichVu { get; private set; } = [];
    public DateOnly NgayChon { get; private set; }

    // Form binding
    [BindProperty] public int IdCaLamViec { get; set; }
    [BindProperty] public int IdDichVu { get; set; }
    [BindProperty] public string? TrieuChung { get; set; }
    [BindProperty] public string? BacSiMongMuonNote { get; set; }

    public async Task OnGetAsync(DateOnly? ngay)
    {
        NgayChon = ngay ?? DateOnly.FromDateTime(DateTime.Now.AddDays(1));
        await TaiDuLieuAsync(NgayChon);
    }

    public async Task<IActionResult> OnPostAsync(DateOnly ngay)
    {
        NgayChon = ngay;

        if (!ModelState.IsValid || IdCaLamViec <= 0 || IdDichVu <= 0)
        {
            await TaiDuLieuAsync(NgayChon);
            TempData["ErrorMessage"] = "Vui lòng chọn đầy đủ ca làm việc và dịch vụ.";
            return Page();
        }

        try
        {
            var command = new TaoLichHenCommand(
                IdCaLamViec,
                IdDichVu,
                IdBenhNhan: null, // benh_nhan tu dat — handler lay tu ICurrentUserService
                IdBacSiMongMuon: null,
                BacSiMongMuonNote,
                TrieuChung);

            var result = await _mediator.Send(command);
            TempData["SuccessMessage"] = $"Đặt lịch thành công! Mã lịch hẹn: {result.MaLichHen}";
            return RedirectToPage("/BenhNhan/DanhSachLichHen");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            await TaiDuLieuAsync(NgayChon);
            return Page();
        }
    }

    private async Task TaiDuLieuAsync(DateOnly ngay)
    {
        var caTask = _mediator.Send(new DanhSachCaLamViecCongKhaiQuery(
            SoTrang: 1,
            KichThuocTrang: 50,
            TuNgay: ngay,
            DenNgay: ngay,
            ConTrong: true));

        var dvTask = _mediator.Send(new DanhSachDichVuQuery(
            SoTrang: 1,
            KichThuocTrang: 100,
            HienThi: true));

        await Task.WhenAll(caTask, dvTask);
        DanhSachCa = caTask.Result;
        DanhSachDichVu = dvTask.Result;
    }
}
