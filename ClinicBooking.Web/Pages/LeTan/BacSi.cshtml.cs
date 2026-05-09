using ClinicBooking.Application.Features.BacSi.Commands.CapNhatBacSi;
using ClinicBooking.Application.Features.BacSi.Commands.TaoBacSi;
using ClinicBooking.Application.Features.BacSi.Commands.XoaBacSi;
using ClinicBooking.Application.Features.BacSi.Dtos;
using ClinicBooking.Application.Features.BacSi.Queries.DanhSachBacSi;
using ClinicBooking.Application.Features.DanhMuc.Dtos;
using ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachChuyenKhoa;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicBooking.Web.Pages.LeTan;

[Authorize(Roles = "le_tan,admin")]
public class BacSiModel : PageModel
{
    private readonly IMediator _mediator;

    public BacSiModel(IMediator mediator) => _mediator = mediator;

    public IReadOnlyList<BacSiResponse> DanhSach { get; private set; } = [];
    public IReadOnlyList<ChuyenKhoaResponse> DanhSachChuyenKhoa { get; private set; } = [];

    [BindProperty] public int? IdBacSi { get; set; }
    [BindProperty] public int IdTaiKhoan { get; set; }
    [BindProperty] public int IdChuyenKhoa { get; set; }
    [BindProperty] public string HoTen { get; set; } = string.Empty;
    [BindProperty] public string? AnhDaiDien { get; set; }
    [BindProperty] public string? BangCap { get; set; }
    [BindProperty] public int? NamKinhNghiem { get; set; }
    [BindProperty] public string? TieuSu { get; set; }
    [BindProperty] public LoaiHopDong LoaiHopDong { get; set; } = LoaiHopDong.NoiTru;
    [BindProperty] public TrangThaiBacSi TrangThai { get; set; } = TrangThaiBacSi.DangLam;

    public int? IdChuyenKhoaLoc { get; private set; }
    public string? TuKhoaLoc { get; private set; }

    public async Task OnGetAsync(int? idChuyenKhoa = null, string? tuKhoa = null)
    {
        IdChuyenKhoaLoc = idChuyenKhoa;
        TuKhoaLoc = tuKhoa;
        await TaiDuLieuAsync(idChuyenKhoa, tuKhoa);
    }

    public async Task<IActionResult> OnPostTaoAsync()
    {
        await _mediator.Send(new TaoBacSiCommand(IdTaiKhoan, IdChuyenKhoa, HoTen, AnhDaiDien, BangCap, NamKinhNghiem, TieuSu, LoaiHopDong.ToString(), TrangThai.ToString()));
        TempData["SuccessMessage"] = "Đã tạo bác sĩ.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostCapNhatAsync()
    {
        if (!IdBacSi.HasValue) return RedirectToPage();
        await _mediator.Send(new CapNhatBacSiCommand(IdBacSi.Value, IdChuyenKhoa, HoTen, AnhDaiDien, BangCap, NamKinhNghiem, TieuSu, LoaiHopDong.ToString(), TrangThai.ToString()));
        TempData["SuccessMessage"] = "Đã cập nhật bác sĩ.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostXoaAsync(int idBacSi)
    {
        await _mediator.Send(new XoaBacSiCommand(idBacSi));
        TempData["SuccessMessage"] = "Đã xóa bác sĩ.";
        return RedirectToPage();
    }

    private async Task TaiDuLieuAsync(int? idChuyenKhoa, string? tuKhoa)
    {
        DanhSachChuyenKhoa = await _mediator.Send(new DanhSachChuyenKhoaQuery(1, 200, true, tuKhoa));
        DanhSach = await _mediator.Send(new DanhSachBacSiQuery(1, 100, idChuyenKhoa, tuKhoa));
    }
}
