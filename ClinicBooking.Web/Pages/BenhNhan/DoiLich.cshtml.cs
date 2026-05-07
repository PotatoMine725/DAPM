using ClinicBooking.Application.Features.LichHen.Commands.DoiLichHen;
using ClinicBooking.Application.Features.LichHen.Dtos;
using ClinicBooking.Application.Features.LichHen.Queries.XemLichHen;
using ClinicBooking.Application.Features.Scheduling.Dtos;
using ClinicBooking.Application.Features.Scheduling.Queries.DanhSachCaLamViecCongKhai;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicBooking.Web.Pages.BenhNhan;

[Authorize(Roles = "benh_nhan")]
public class DoiLichModel : PageModel
{
    private readonly IMediator _mediator;

    public DoiLichModel(IMediator mediator) => _mediator = mediator;

    public LichHenResponse LichHen { get; private set; } = default!;
    public IReadOnlyList<CaLamViecPublicResponse> DanhSachCa { get; private set; } = [];
    public bool CanSubmit => IdCaLamViecMoi > 0;
    public string TenCaDaChon => CaDaChon is null ? "—" : $"{CaDaChon.NgayLamViec:dd/MM/yyyy} · {CaDaChon.GioBatDau:HH:mm}-{CaDaChon.GioKetThuc:HH:mm}";
    public string NgayCaDaChon => CaDaChon is null ? "—" : CaDaChon.NgayLamViec.ToString("dd/MM/yyyy");
    public string MaPhongDaChon => CaDaChon?.MaPhong ?? "—";
    public string TenChuyenKhoaDaChon => CaDaChon?.TenChuyenKhoa ?? "—";

    [BindProperty]
    public int IdCaLamViecMoi { get; set; }

    [BindProperty]
    public string? LyDo { get; set; }

    private CaLamViecPublicResponse? CaDaChon => DanhSachCa.FirstOrDefault(x => x.IdCaLamViec == IdCaLamViecMoi);

    public async Task OnGetAsync(int idLichHen)
    {
        LichHen = await _mediator.Send(new XemLichHenQuery(idLichHen));
        await TaiCaLamViecAsync(LichHen.IdCaLamViec);
    }

    public async Task<IActionResult> OnPostAsync(int idLichHen)
    {
        LichHen = await _mediator.Send(new XemLichHenQuery(idLichHen));
        await TaiCaLamViecAsync(LichHen.IdCaLamViec);

        if (!ModelState.IsValid || IdCaLamViecMoi <= 0)
        {
            TempData["ErrorMessage"] = "Vui lòng chọn ca làm việc mới hợp lệ.";
            return Page();
        }

        try
        {
            await _mediator.Send(new DoiLichHenCommand(
                LichHen.IdLichHen,
                IdCaLamViecMoi,
                LichHen.IdDichVu,
                LichHen.IdBacSiMongMuon,
                LichHen.BacSiMongMuonNote,
                LichHen.TrieuChung,
                LyDo));

            TempData["SuccessMessage"] = "Đã đổi lịch hẹn thành công.";
            return RedirectToPage("/BenhNhan/LichHen", new { idLichHen = LichHen.IdLichHen });
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return Page();
        }
    }

    private async Task TaiCaLamViecAsync(int idCaHienTai)
    {
        var caHienTai = await _mediator.Send(new DanhSachCaLamViecCongKhaiQuery(
            SoTrang: 1,
            KichThuocTrang: 50,
            TuNgay: DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            DenNgay: DateOnly.FromDateTime(DateTime.Now.AddDays(60)),
            ConTrong: true));

        DanhSachCa = caHienTai
            .Where(x => x.IdCaLamViec != idCaHienTai)
            .OrderBy(x => x.NgayLamViec)
            .ThenBy(x => x.GioBatDau)
            .ToList();
    }
}
