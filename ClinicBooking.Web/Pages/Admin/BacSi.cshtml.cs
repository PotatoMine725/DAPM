using ClinicBooking.Application.Common.Constants;
using ClinicBooking.Application.Features.DanhMuc.Dtos;
using ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachChuyenKhoa;
using ClinicBooking.Application.Features.Doctors.Commands.CapNhatBacSi;
using ClinicBooking.Application.Features.Doctors.Commands.TaoBacSi;
using ClinicBooking.Application.Features.Doctors.Dtos;
using ClinicBooking.Application.Features.Doctors.Queries.DanhSachBacSiAdmin;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicBooking.Web.Pages.Admin;

[Authorize(Roles = VaiTroConstants.Admin)]
public class BacSiModel : PageModel
{
    private readonly IMediator _mediator;

    public BacSiModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public IReadOnlyList<BacSiAdminResponse> DanhSach { get; private set; } = [];
    public IReadOnlyList<ChuyenKhoaResponse> DanhSachChuyenKhoa { get; private set; } = [];

    public int? IdChuyenKhoaLoc { get; private set; }
    public LoaiHopDong? LoaiHopDongLoc { get; private set; }
    public TrangThaiBacSi? TrangThaiLoc { get; private set; }
    public string? TuKhoa { get; private set; }

    [BindProperty] public string TenDangNhap { get; set; } = string.Empty;
    [BindProperty] public string Email { get; set; } = string.Empty;
    [BindProperty] public string SoDienThoai { get; set; } = string.Empty;
    [BindProperty] public string MatKhau { get; set; } = string.Empty;
    [BindProperty] public string XacNhanMatKhau { get; set; } = string.Empty;
    [BindProperty] public string HoTen { get; set; } = string.Empty;
    [BindProperty] public int IdChuyenKhoaInput { get; set; }
    [BindProperty] public LoaiHopDong LoaiHopDongInput { get; set; } = LoaiHopDong.NoiTru;
    [BindProperty] public string? BangCap { get; set; }
    [BindProperty] public int? NamKinhNghiem { get; set; }
    [BindProperty] public string? TieuSu { get; set; }

    [BindProperty] public int IdBacSiSua { get; set; }
    [BindProperty] public TrangThaiBacSi TrangThaiInput { get; set; } = TrangThaiBacSi.DangLam;

    public async Task OnGetAsync(int? idChuyenKhoa = null, LoaiHopDong? loaiHopDong = null, TrangThaiBacSi? trangThai = null, string? tuKhoa = null)
    {
        IdChuyenKhoaLoc = idChuyenKhoa;
        LoaiHopDongLoc = loaiHopDong;
        TrangThaiLoc = trangThai;
        TuKhoa = tuKhoa;
        await TaiDuLieuAsync();
    }

    public async Task<IActionResult> OnPostTaoAsync()
    {
        try
        {
            await _mediator.Send(new TaoBacSiCommand(
                TenDangNhap.Trim(),
                Email.Trim(),
                SoDienThoai.Trim(),
                MatKhau,
                HoTen.Trim(),
                IdChuyenKhoaInput,
                LoaiHopDongInput,
                BangCap,
                NamKinhNghiem,
                TieuSu));
            TempData["SuccessMessage"] = $"Đã tạo bác sĩ {HoTen}.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostCapNhatAsync()
    {
        try
        {
            await _mediator.Send(new CapNhatBacSiCommand(
                IdBacSiSua,
                HoTen.Trim(),
                IdChuyenKhoaInput,
                LoaiHopDongInput,
                TrangThaiInput,
                BangCap,
                NamKinhNghiem,
                TieuSu,
                string.IsNullOrWhiteSpace(Email) ? null : Email.Trim(),
                string.IsNullOrWhiteSpace(SoDienThoai) ? null : SoDienThoai.Trim()));
            TempData["SuccessMessage"] = "Đã cập nhật bác sĩ.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDoiTrangThaiAsync(int idBacSi, TrangThaiBacSi trangThai)
    {
        try
        {
            var current = await _mediator.Send(new DanhSachBacSiAdminQuery(SoTrang: 1, KichThuocTrang: 500));
            var bsCurr = current.FirstOrDefault(x => x.IdBacSi == idBacSi);
            if (bsCurr is null)
            {
                TempData["ErrorMessage"] = "Khong tim thay bac si.";
                return RedirectToPage();
            }
            await _mediator.Send(new CapNhatBacSiCommand(
                idBacSi,
                bsCurr.HoTen,
                bsCurr.IdChuyenKhoa,
                Enum.Parse<LoaiHopDong>(bsCurr.LoaiHopDong),
                trangThai,
                bsCurr.BangCap,
                bsCurr.NamKinhNghiem,
                bsCurr.TieuSu,
                null,
                null));
            TempData["SuccessMessage"] = $"Đã đổi trạng thái BS {bsCurr.HoTen}.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToPage();
    }

    private async Task TaiDuLieuAsync()
    {
        DanhSachChuyenKhoa = await _mediator.Send(new DanhSachChuyenKhoaQuery(1, 200, true, null));
        DanhSach = await _mediator.Send(new DanhSachBacSiAdminQuery(
            SoTrang: 1,
            KichThuocTrang: 200,
            IdChuyenKhoa: IdChuyenKhoaLoc,
            LoaiHopDong: LoaiHopDongLoc,
            TrangThai: TrangThaiLoc,
            TuKhoa: TuKhoa));
    }
}
