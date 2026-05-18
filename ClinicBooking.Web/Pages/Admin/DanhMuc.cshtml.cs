using ClinicBooking.Application.Features.DanhMuc.Commands.CapNhatChuyenKhoa;
using ClinicBooking.Application.Features.DanhMuc.Commands.CapNhatDinhNghiaCa;
using ClinicBooking.Application.Features.DanhMuc.Commands.CapNhatDichVu;
using ClinicBooking.Application.Features.DanhMuc.Commands.CapNhatPhong;
using ClinicBooking.Application.Features.DanhMuc.Commands.TaoChuyenKhoa;
using ClinicBooking.Application.Features.DanhMuc.Commands.TaoDinhNghiaCa;
using ClinicBooking.Application.Features.DanhMuc.Commands.TaoDichVu;
using ClinicBooking.Application.Features.DanhMuc.Commands.TaoPhong;
using ClinicBooking.Application.Features.DanhMuc.Commands.XoaChuyenKhoa;
using ClinicBooking.Application.Features.DanhMuc.Commands.XoaDinhNghiaCa;
using ClinicBooking.Application.Features.DanhMuc.Commands.XoaDichVu;
using ClinicBooking.Application.Features.DanhMuc.Commands.XoaPhong;
using ClinicBooking.Application.Features.DanhMuc.Dtos;
using ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachChuyenKhoa;
using ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachDinhNghiaCa;
using ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachDichVu;
using ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachPhong;
using ClinicBooking.Application.Features.Thuoc.Commands.CapNhatThuoc;
using ClinicBooking.Application.Features.Thuoc.Commands.TaoThuoc;
using ClinicBooking.Application.Features.Thuoc.Commands.XoaThuoc;
using ClinicBooking.Application.Features.Thuoc.Dtos;
using ClinicBooking.Application.Features.Thuoc.Queries.DanhSachThuoc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicBooking.Web.Pages.Admin;

[Authorize(Roles = "admin")]
public class DanhMucModel : PageModel
{
    private readonly IMediator _mediator;

    public DanhMucModel(IMediator mediator) => _mediator = mediator;

    public IReadOnlyList<ChuyenKhoaResponse> ChuyenKhoa { get; private set; } = [];
    public IReadOnlyList<DichVuResponse> DichVu { get; private set; } = [];
    public IReadOnlyList<PhongResponse> Phong { get; private set; } = [];
    public IReadOnlyList<DinhNghiaCaResponse> DinhNghiaCa { get; private set; } = [];
    public IReadOnlyList<ThuocResponse> Thuoc { get; private set; } = [];

    [BindProperty] public int? IdChuyenKhoa { get; set; }
    [BindProperty] public string? TenChuyenKhoa { get; set; }
    [BindProperty] public string? MoTaChuyenKhoa { get; set; }
    [BindProperty] public int ThoiGianSlotMacDinh { get; set; } = 15;
    [BindProperty] public bool HienThiChuyenKhoa { get; set; } = true;

    [BindProperty] public int? IdDichVu { get; set; }
    [BindProperty] public string? TenDichVu { get; set; }
    [BindProperty] public int? IdChuyenKhoaDichVu { get; set; }
    [BindProperty] public string? MoTaDichVu { get; set; }
    [BindProperty] public int ThoiGianUocTinh { get; set; } = 15;
    [BindProperty] public bool HienThiDichVu { get; set; } = true;

    [BindProperty] public int? IdPhong { get; set; }
    [BindProperty] public string? MaPhong { get; set; }
    [BindProperty] public string? TenPhong { get; set; }
    [BindProperty] public int? SucChua { get; set; }
    [BindProperty] public string? TrangBi { get; set; }
    [BindProperty] public bool TrangThaiPhong { get; set; } = true;

    [BindProperty] public int? IdDinhNghiaCa { get; set; }
    [BindProperty] public string? TenCa { get; set; }
    [BindProperty] public TimeOnly GioBatDauMacDinh { get; set; } = new(8, 0);
    [BindProperty] public TimeOnly GioKetThucMacDinh { get; set; } = new(11, 0);
    [BindProperty] public string? MoTaCa { get; set; }
    [BindProperty] public bool TrangThaiCa { get; set; } = true;

    [BindProperty] public int? IdThuoc { get; set; }
    [BindProperty] public string? TenThuoc { get; set; }
    [BindProperty] public string? HoatChat { get; set; }
    [BindProperty] public string? DonVi { get; set; }
    [BindProperty] public string? GhiChuThuoc { get; set; }
    [BindProperty(SupportsGet = true)] public string? TuKhoaThuoc { get; set; }

    public async Task OnGetAsync() => await TaiDuLieuAsync();

    public async Task<IActionResult> OnPostTaoChuyenKhoaAsync()
    {
        await _mediator.Send(new TaoChuyenKhoaCommand(TenChuyenKhoa ?? string.Empty, MoTaChuyenKhoa, ThoiGianSlotMacDinh, null, null, HienThiChuyenKhoa));
        TempData["SuccessMessage"] = "Đã tạo chuyên khoa.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostCapNhatChuyenKhoaAsync()
    {
        if (!IdChuyenKhoa.HasValue) return RedirectToPage();
        await _mediator.Send(new CapNhatChuyenKhoaCommand(IdChuyenKhoa.Value, TenChuyenKhoa ?? string.Empty, MoTaChuyenKhoa, ThoiGianSlotMacDinh, null, null, HienThiChuyenKhoa));
        TempData["SuccessMessage"] = "Đã cập nhật chuyên khoa.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostXoaChuyenKhoaAsync(int idChuyenKhoa)
    {
        await _mediator.Send(new XoaChuyenKhoaCommand(idChuyenKhoa));
        TempData["SuccessMessage"] = "Đã xóa chuyên khoa.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostTaoDichVuAsync()
    {
        await _mediator.Send(new TaoDichVuCommand(IdChuyenKhoaDichVu ?? 0, TenDichVu ?? string.Empty, MoTaDichVu, ThoiGianUocTinh, HienThiDichVu));
        TempData["SuccessMessage"] = "Đã tạo dịch vụ.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostCapNhatDichVuAsync()
    {
        if (!IdDichVu.HasValue) return RedirectToPage();
        await _mediator.Send(new CapNhatDichVuCommand(IdDichVu.Value, IdChuyenKhoaDichVu ?? 0, TenDichVu ?? string.Empty, MoTaDichVu, ThoiGianUocTinh, HienThiDichVu));
        TempData["SuccessMessage"] = "Đã cập nhật dịch vụ.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostXoaDichVuAsync(int idDichVu)
    {
        await _mediator.Send(new XoaDichVuCommand(idDichVu));
        TempData["SuccessMessage"] = "Đã xóa dịch vụ.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostTaoPhongAsync()
    {
        await _mediator.Send(new TaoPhongCommand(MaPhong ?? string.Empty, TenPhong ?? string.Empty, SucChua, TrangBi, TrangThaiPhong));
        TempData["SuccessMessage"] = "Đã tạo phòng.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostCapNhatPhongAsync()
    {
        if (!IdPhong.HasValue) return RedirectToPage();
        await _mediator.Send(new CapNhatPhongCommand(IdPhong.Value, MaPhong ?? string.Empty, TenPhong ?? string.Empty, SucChua, TrangBi, TrangThaiPhong));
        TempData["SuccessMessage"] = "Đã cập nhật phòng.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostXoaPhongAsync(int idPhong)
    {
        await _mediator.Send(new XoaPhongCommand(idPhong));
        TempData["SuccessMessage"] = "Đã xóa phòng.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostTaoDinhNghiaCaAsync()
    {
        await _mediator.Send(new TaoDinhNghiaCaCommand(TenCa ?? string.Empty, GioBatDauMacDinh, GioKetThucMacDinh, MoTaCa, TrangThaiCa));
        TempData["SuccessMessage"] = "Đã tạo định nghĩa ca.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostCapNhatDinhNghiaCaAsync()
    {
        if (!IdDinhNghiaCa.HasValue) return RedirectToPage();
        await _mediator.Send(new CapNhatDinhNghiaCaCommand(IdDinhNghiaCa.Value, TenCa ?? string.Empty, GioBatDauMacDinh, GioKetThucMacDinh, MoTaCa, TrangThaiCa));
        TempData["SuccessMessage"] = "Đã cập nhật định nghĩa ca.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostXoaDinhNghiaCaAsync(int idDinhNghiaCa)
    {
        await _mediator.Send(new XoaDinhNghiaCaCommand(idDinhNghiaCa));
        TempData["SuccessMessage"] = "Đã xóa định nghĩa ca.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostTaoThuocAsync()
    {
        await _mediator.Send(new TaoThuocCommand(TenThuoc ?? string.Empty, HoatChat, DonVi, GhiChuThuoc));
        TempData["SuccessMessage"] = "Đã tạo thuốc.";
        return RedirectToPage(new { TuKhoaThuoc });
    }

    public async Task<IActionResult> OnPostCapNhatThuocAsync()
    {
        if (!IdThuoc.HasValue) return RedirectToPage(new { TuKhoaThuoc });

        await _mediator.Send(new CapNhatThuocCommand(
            IdThuoc.Value,
            TenThuoc ?? string.Empty,
            HoatChat,
            DonVi,
            GhiChuThuoc));
        TempData["SuccessMessage"] = "Đã cập nhật thuốc.";
        return RedirectToPage(new { TuKhoaThuoc });
    }

    public async Task<IActionResult> OnPostXoaThuocAsync(int idThuoc)
    {
        await _mediator.Send(new XoaThuocCommand(idThuoc));
        TempData["SuccessMessage"] = "Đã xóa thuốc.";
        return RedirectToPage(new { TuKhoaThuoc });
    }

    private async Task TaiDuLieuAsync()
    {
        ChuyenKhoa = await _mediator.Send(new DanhSachChuyenKhoaQuery(1, 200, null, null));
        DichVu = await _mediator.Send(new DanhSachDichVuQuery(1, 200, null, null, null));
        Phong = await _mediator.Send(new DanhSachPhongQuery(1, 200, null, null));
        DinhNghiaCa = await _mediator.Send(new DanhSachDinhNghiaCaQuery(1, 200, null, null));
        Thuoc = await _mediator.Send(new DanhSachThuocQuery(1, 200, TuKhoaThuoc));
    }
}
