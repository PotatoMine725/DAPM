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

    public IReadOnlyList<CaLamViecPublicResponse> DanhSachCa { get; private set; } = [];
    public IReadOnlyList<DichVuResponse> DanhSachDichVu { get; private set; } = [];
    [BindProperty(SupportsGet = true)] public DateOnly NgayChon { get; set; }
    public string ChuyenKhoaHienTai => string.IsNullOrWhiteSpace(TenChuyenKhoaDaChon) ? "Nội tổng quát" : TenChuyenKhoaDaChon;
    public string TenDichVuDaChon => string.IsNullOrWhiteSpace(TenDichVu) ? "—" : TenDichVu;
    public bool CanSubmit => IdDichVu > 0;

    [BindProperty] public int IdDichVu { get; set; }
    [BindProperty] public TimeOnly GioMongMuon { get; set; } = new TimeOnly(8, 0);
    [BindProperty] public string? TrieuChung { get; set; }
    [BindProperty] public string? GhiChu { get; set; }
    [BindProperty] public string? Otp { get; set; }

    public string? TenChuyenKhoaDaChon { get; private set; }
    public string? TenDichVu { get; private set; }

    public async Task OnGetAsync(DateOnly? ngay)
    {
        NgayChon = ngay ?? DateOnly.FromDateTime(DateTime.Now.AddDays(1));
        await TaiDuLieuAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await TaiDuLieuAsync();

        if (!ModelState.IsValid || IdDichVu <= 0)
        {
            TempData["ErrorMessage"] = "Vui lòng chọn dịch vụ hợp lệ.";
            return Page();
        }

        try
        {
            var command = new TaoLichHenCommand(
                NgayChon,
                GioMongMuon,
                IdDichVu,
                IdBenhNhan: null,
                IdBacSiMongMuon: null,
                BacSiMongMuonNote: GhiChu,
                TrieuChung);

            var result = await _mediator.Send(command);
            TempData["SuccessMessage"] = $"Đặt lịch thành công! Mã lịch hẹn: {result.MaLichHen}. Hệ thống sẽ tự sắp xếp ca khám phù hợp và gửi thông báo sau khi hoàn tất.";
            return RedirectToPage("/BenhNhan/DanhSachLichHen");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return Page();
        }
    }

    public async Task<IActionResult> OnPostGuiOtpAsync()
    {
        await TaiDuLieuAsync();
        TempData["SuccessMessage"] = "Đã nhận yêu cầu. Hệ thống sẽ gửi OTP sau khi hoàn tất bước xác nhận trước đặt lịch.";
        return Page();
    }

    private async Task TaiDuLieuAsync()
    {
        DanhSachCa = await _mediator.Send(new DanhSachCaLamViecCongKhaiQuery(
            SoTrang: 1,
            KichThuocTrang: 50,
            TuNgay: NgayChon,
            DenNgay: NgayChon,
            ConTrong: true));

        DanhSachDichVu = await _mediator.Send(new DanhSachDichVuQuery(
            SoTrang: 1,
            KichThuocTrang: 100,
            HienThi: true));
    }
}
