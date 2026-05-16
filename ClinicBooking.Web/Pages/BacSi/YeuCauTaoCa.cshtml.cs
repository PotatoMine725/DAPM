using ClinicBooking.Application.Features.DanhMuc.Dtos;
using ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachChuyenKhoa;
using ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachDinhNghiaCa;
using ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachPhong;
using ClinicBooking.Application.Features.Scheduling.Commands.TaoCaLamViec;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicBooking.Web.Pages.BacSi;

[Authorize(Roles = "bac_si")]
public class YeuCauTaoCaModel : PageModel
{
    private readonly IMediator _mediator;

    public YeuCauTaoCaModel(IMediator mediator) => _mediator = mediator;

    public IReadOnlyList<ChuyenKhoaResponse> DanhSachChuyenKhoa { get; private set; } = [];
    public IReadOnlyList<PhongResponse> DanhSachPhong { get; private set; } = [];
    public IReadOnlyList<DinhNghiaCaResponse> DanhSachDinhNghiaCa { get; private set; } = [];

    [BindProperty] public int IdChuyenKhoa { get; set; }
    [BindProperty] public int IdPhong { get; set; }
    [BindProperty] public int IdDinhNghiaCa { get; set; }
    [BindProperty] public DateOnly NgayLamViec { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow.Date);
    [BindProperty] public TimeOnly GioBatDau { get; set; } = new(8, 0);
    [BindProperty] public TimeOnly GioKetThuc { get; set; } = new(11, 0);
    [BindProperty] public int ThoiGianSlot { get; set; } = 15;
    [BindProperty] public int SoSlotToiDa { get; set; } = 12;

    public async Task OnGetAsync()
    {
        await TaiDuLieuAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            await _mediator.Send(new TaoCaLamViecCommand(
                IdBacSi: 0,
                IdPhong: IdPhong,
                IdChuyenKhoa: IdChuyenKhoa,
                IdDinhNghiaCa: IdDinhNghiaCa,
                NgayLamViec: NgayLamViec,
                GioBatDau: GioBatDau,
                GioKetThuc: GioKetThuc,
                ThoiGianSlot: ThoiGianSlot,
                SoSlotToiDa: SoSlotToiDa,
                IdBacSiYeuCau: null));
            TempData["SuccessMessage"] = "Đã gửi yêu cầu tạo ca.";
            return RedirectToPage("/BacSi/DanhSach");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            await TaiDuLieuAsync();
            return Page();
        }
    }

    private async Task TaiDuLieuAsync()
    {
        DanhSachChuyenKhoa = await _mediator.Send(new DanhSachChuyenKhoaQuery(1, 200, true, null));
        DanhSachPhong = await _mediator.Send(new DanhSachPhongQuery(1, 200, true, null));
        DanhSachDinhNghiaCa = await _mediator.Send(new DanhSachDinhNghiaCaQuery(1, 200, true, null));
    }
}
