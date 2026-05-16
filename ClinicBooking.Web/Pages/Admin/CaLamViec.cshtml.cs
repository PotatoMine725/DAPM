using ClinicBooking.Application.Common.Constants;
using ClinicBooking.Application.Features.DanhMuc.Dtos;
using ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachChuyenKhoa;
using ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachDinhNghiaCa;
using ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachPhong;
using ClinicBooking.Application.Features.Doctors.Dtos;
using ClinicBooking.Application.Features.Doctors.Queries.DanhSachBacSiAdmin;
using ClinicBooking.Application.Features.Scheduling.Commands.TaoCaLamViec;
using ClinicBooking.Application.Features.Scheduling.Commands.XoaCaLamViec;
using ClinicBooking.Application.Features.Scheduling.Dtos;
using ClinicBooking.Application.Features.Scheduling.Queries.DanhSachCaLamViecChoDuyet;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicBooking.Web.Pages.Admin;

[Authorize(Roles = VaiTroConstants.Admin)]
public class CaLamViecModel : PageModel
{
    private readonly IMediator _mediator;

    public CaLamViecModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public IReadOnlyList<CaLamViecAdminResponse> DanhSach { get; private set; } = [];
    public IReadOnlyList<BacSiAdminResponse> DanhSachBacSi { get; private set; } = [];
    public IReadOnlyList<PhongResponse> DanhSachPhong { get; private set; } = [];
    public IReadOnlyList<ChuyenKhoaResponse> DanhSachChuyenKhoa { get; private set; } = [];
    public IReadOnlyList<DinhNghiaCaResponse> DanhSachDinhNghiaCa { get; private set; } = [];

    public int? IdBacSiLoc { get; private set; }
    public int? IdPhongLoc { get; private set; }
    public int? IdChuyenKhoaLoc { get; private set; }
    public TrangThaiDuyetCa? TrangThaiLoc { get; private set; }
    public DateOnly? TuNgayLoc { get; private set; }
    public DateOnly? DenNgayLoc { get; private set; }

    [BindProperty] public int IdBacSiInput { get; set; }
    [BindProperty] public int IdPhongInput { get; set; }
    [BindProperty] public int IdChuyenKhoaInput { get; set; }
    [BindProperty] public int IdDinhNghiaCaInput { get; set; }
    [BindProperty] public DateOnly NgayLamViecInput { get; set; }
    [BindProperty] public TimeOnly GioBatDauInput { get; set; }
    [BindProperty] public TimeOnly GioKetThucInput { get; set; }
    [BindProperty] public int ThoiGianSlotInput { get; set; } = 20;
    [BindProperty] public int SoSlotToiDaInput { get; set; } = 15;

    [BindProperty] public int IdCaXoa { get; set; }

    public async Task OnGetAsync(
        int? idBacSi = null,
        int? idPhong = null,
        int? idChuyenKhoa = null,
        TrangThaiDuyetCa? trangThai = null,
        DateOnly? tuNgay = null,
        DateOnly? denNgay = null)
    {
        IdBacSiLoc = idBacSi;
        IdPhongLoc = idPhong;
        IdChuyenKhoaLoc = idChuyenKhoa;
        TrangThaiLoc = trangThai;
        TuNgayLoc = tuNgay;
        DenNgayLoc = denNgay;
        await TaiDuLieuAsync();
    }

    public async Task<IActionResult> OnPostTaoAsync()
    {
        if (GioBatDauInput >= GioKetThucInput)
        {
            TempData["ErrorMessage"] = "Giờ kết thúc phải sau giờ bắt đầu.";
            return RedirectToPage();
        }
        try
        {
            await _mediator.Send(new TaoCaLamViecCommand(
                IdBacSiInput,
                IdPhongInput,
                IdChuyenKhoaInput,
                IdDinhNghiaCaInput,
                NgayLamViecInput,
                GioBatDauInput,
                GioKetThucInput,
                ThoiGianSlotInput,
                SoSlotToiDaInput));
            TempData["SuccessMessage"] = "Đã tạo ca làm việc.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostXoaAsync()
    {
        try
        {
            await _mediator.Send(new XoaCaLamViecCommand(IdCaXoa));
            TempData["SuccessMessage"] = "Đã xoá ca làm việc.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToPage();
    }

    private async Task TaiDuLieuAsync()
    {
        DanhSachBacSi = await _mediator.Send(new DanhSachBacSiAdminQuery(1, 500));
        DanhSachPhong = await _mediator.Send(new DanhSachPhongQuery(1, 200, true, null));
        DanhSachChuyenKhoa = await _mediator.Send(new DanhSachChuyenKhoaQuery(1, 200, true, null));
        DanhSachDinhNghiaCa = await _mediator.Send(new DanhSachDinhNghiaCaQuery(1, 50, true, null));

        DanhSach = await _mediator.Send(new DanhSachCaLamViecChoDuyetQuery(
            SoTrang: 1,
            KichThuocTrang: 200,
            TrangThaiDuyet: TrangThaiLoc,
            NguonTaoCa: null,
            IdChuyenKhoa: IdChuyenKhoaLoc,
            IdBacSi: IdBacSiLoc,
            IdPhong: IdPhongLoc,
            TuNgay: TuNgayLoc,
            DenNgay: DenNgayLoc));
    }
}
