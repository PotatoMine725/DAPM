using ClinicBooking.Application.Features.BenhNhan.Commands.CapNhatThongTinBenhNhan;
using ClinicBooking.Application.Features.BenhNhan.Dtos;
using ClinicBooking.Application.Features.BenhNhan.Queries.DanhSachBenhNhan;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicBooking.Web.Pages.LeTan;

[Authorize(Roles = "le_tan,admin")]
public class HoSoBenhNhanModel : PageModel
{
    private readonly IMediator _mediator;

    public HoSoBenhNhanModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    [BindProperty(SupportsGet = true)]
    public string? TuKhoa { get; set; }

    [BindProperty(SupportsGet = true)]
    public int Trang { get; set; } = 1;

    public IReadOnlyList<BenhNhanResponse> DanhSach { get; private set; } = [];
    public bool CoTrangSau { get; private set; }

    private const int KichThuocTrang = 20;

    public async Task OnGetAsync()
    {
        var ketQua = await _mediator.Send(new DanhSachBenhNhanQuery(
            SoTrang: Trang,
            KichThuocTrang: KichThuocTrang + 1,
            TuKhoa: TuKhoa));

        CoTrangSau = ketQua.Count > KichThuocTrang;
        DanhSach = ketQua.Take(KichThuocTrang).ToList();
    }

    public async Task<IActionResult> OnPostCapNhatAsync(
        int idBenhNhan,
        string hoTen,
        string? ngaySinh,
        GioiTinh? gioiTinh,
        string? cccd,
        string? diaChi)
    {
        if (string.IsNullOrWhiteSpace(hoTen))
        {
            TempData["ErrorMessage"] = "Họ tên không được để trống.";
            return RedirectToPage(new { TuKhoa, Trang });
        }

        DateOnly? ngaySinhParsed = null;
        if (!string.IsNullOrWhiteSpace(ngaySinh) && DateOnly.TryParse(ngaySinh, out var d))
            ngaySinhParsed = d;

        try
        {
            await _mediator.Send(new CapNhatThongTinBenhNhanCommand(
                idBenhNhan, hoTen, ngaySinhParsed, gioiTinh, cccd, diaChi));
            TempData["SuccessMessage"] = "Đã cập nhật hồ sơ bệnh nhân.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToPage(new { TuKhoa, Trang });
    }
}
