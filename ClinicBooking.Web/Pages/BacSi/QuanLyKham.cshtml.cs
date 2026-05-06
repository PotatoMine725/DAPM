using ClinicBooking.Application.Common.Constants;
using ClinicBooking.Application.Features.HoSoKham.Commands.CapNhatHoSoKham;
using ClinicBooking.Application.Features.HoSoKham.Commands.TaoHoSoKham;
using ClinicBooking.Application.Features.HoSoKham.Dtos;
using ClinicBooking.Application.Features.HoSoKham.Queries.LayHoSoKhamById;
using ClinicBooking.Application.Features.Thuoc.Dtos;
using ClinicBooking.Application.Features.Thuoc.Queries.DanhSachThuoc;
using ClinicBooking.Application.Features.ToaThuoc.Commands.CapNhatToaThuoc;
using ClinicBooking.Application.Features.ToaThuoc.Commands.TaoToaThuoc;
using ClinicBooking.Application.Features.ToaThuoc.Dtos;
using ClinicBooking.Application.Features.ToaThuoc.Queries.LayToaTheoHoSoKham;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicBooking.Web.Pages.BacSi;

[Authorize(Roles = VaiTroConstants.BacSi)]
public class QuanLyKhamModel : PageModel
{
    private readonly IMediator _mediator;

    public QuanLyKhamModel(IMediator mediator) => _mediator = mediator;

    [BindProperty(SupportsGet = true)]
    public int? IdHoSoKham { get; set; }

    [BindProperty]
    public HoSoKhamFormInput HoSo { get; set; } = new();

    [BindProperty]
    public ToaThuocFormInput ToaThuoc { get; set; } = new();

    public HoSoKhamResponse? HoSoHienTai { get; private set; }
    public IReadOnlyList<ToaThuocResponse> ToaHienTai { get; private set; } = [];
    public IReadOnlyList<ThuocResponse> DanhSachThuoc { get; private set; } = [];

    public async Task OnGetAsync()
    {
        await TaiDuLieuTrangAsync();
    }

    public async Task<IActionResult> OnPostTaoHoSoAsync()
    {
        try
        {
            var id = await _mediator.Send(new TaoHoSoKhamCommand(
                HoSo.IdLichHen,
                HoSo.ChanDoan,
                HoSo.KetQuaKham,
                HoSo.GhiChu));
            TempData["SuccessMessage"] = "Da tao ho so kham.";
            return RedirectToPage(new { idHoSoKham = id });
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToPage(new { idHoSoKham = IdHoSoKham });
        }
    }

    public async Task<IActionResult> OnPostCapNhatHoSoAsync()
    {
        try
        {
            await _mediator.Send(new CapNhatHoSoKhamCommand(
                HoSo.IdHoSoKham,
                HoSo.ChanDoan,
                HoSo.KetQuaKham,
                HoSo.GhiChu));
            TempData["SuccessMessage"] = "Da cap nhat ho so kham.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToPage(new { idHoSoKham = HoSo.IdHoSoKham });
    }

    public async Task<IActionResult> OnPostKeToaAsync()
    {
        try
        {
            var danhSachThuoc = ToaThuoc.DanhSachThuoc
                .Where(x => x.IdThuoc > 0)
                .Select(x => new ToaThuocChiTietInput(
                    x.IdThuoc,
                    x.LieuLuong,
                    x.CachDung,
                    x.SoNgayDung,
                    x.GhiChu))
                .ToList();

            if (ToaThuoc.CapNhatToaDangCo)
            {
                await _mediator.Send(new CapNhatToaThuocCommand(ToaThuoc.IdHoSoKham, danhSachThuoc));
                TempData["SuccessMessage"] = "Da cap nhat toa thuoc.";
            }
            else
            {
                await _mediator.Send(new TaoToaThuocCommand(ToaThuoc.IdHoSoKham, danhSachThuoc));
                TempData["SuccessMessage"] = "Da ke toa thuoc.";
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToPage(new { idHoSoKham = ToaThuoc.IdHoSoKham });
    }

    private async Task TaiDuLieuTrangAsync()
    {
        DanhSachThuoc = await _mediator.Send(new DanhSachThuocQuery(1, 100, null));

        if (!IdHoSoKham.HasValue)
        {
            return;
        }

        HoSoHienTai = await _mediator.Send(new LayHoSoKhamByIdQuery(IdHoSoKham.Value));
        ToaHienTai = await _mediator.Send(new LayToaTheoHoSoKhamQuery(IdHoSoKham.Value));
    }

    public sealed class HoSoKhamFormInput
    {
        public int IdHoSoKham { get; set; }
        public int IdLichHen { get; set; }
        public string? ChanDoan { get; set; }
        public string? KetQuaKham { get; set; }
        public string? GhiChu { get; set; }
    }

    public sealed class ToaThuocFormInput
    {
        public int IdHoSoKham { get; set; }
        public bool CapNhatToaDangCo { get; set; }
        public List<ToaThuocLineInput> DanhSachThuoc { get; set; } =
            Enumerable.Range(0, 5).Select(_ => new ToaThuocLineInput()).ToList();
    }

    public sealed class ToaThuocLineInput
    {
        public int IdThuoc { get; set; }
        public string? LieuLuong { get; set; }
        public string? CachDung { get; set; }
        public int? SoNgayDung { get; set; }
        public string? GhiChu { get; set; }
    }
}
