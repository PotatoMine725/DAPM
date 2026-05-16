using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Constants;
using ClinicBooking.Application.Features.DanhMuc.Dtos;
using ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachDinhNghiaCa;
using ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachPhong;
using ClinicBooking.Application.Features.Doctors.Dtos;
using ClinicBooking.Application.Features.Doctors.Queries.DanhSachBacSiCongKhai;
using ClinicBooking.Application.Features.Scheduling.Commands.SinhCaLamViecTuLichNoiTru;
using ClinicBooking.Application.Features.Scheduling.Commands.TaoLichNoiTru;
using ClinicBooking.Application.Features.Scheduling.Commands.VoHieuLichNoiTru;
using ClinicBooking.Application.Features.Scheduling.Dtos;
using ClinicBooking.Application.Features.Scheduling.Queries.DanhSachLichNoiTruTheoBacSi;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicBooking.Web.Pages.Admin;

[Authorize(Roles = VaiTroConstants.Admin)]
public class LichNoiTruModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public LichNoiTruModel(IMediator mediator, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    public IReadOnlyList<BacSiPublicResponse> DanhSachBacSi { get; private set; } = [];
    public IReadOnlyList<LichNoiTruResponse> DanhSachLich { get; private set; } = [];
    public IReadOnlyList<DinhNghiaCaResponse> DanhSachDinhNghiaCa { get; private set; } = [];
    public IReadOnlyList<PhongResponse> DanhSachPhong { get; private set; } = [];

    public int? IdBacSiDangChon { get; private set; }
    public BacSiPublicResponse? BacSiDangChon { get; private set; }

    [BindProperty] public int IdBacSiInput { get; set; }
    [BindProperty] public int IdPhongInput { get; set; }
    [BindProperty] public int IdDinhNghiaCaInput { get; set; }
    [BindProperty] public int NgayTrongTuanInput { get; set; }
    [BindProperty] public DateOnly NgayApDungInput { get; set; }
    [BindProperty] public DateOnly? NgayKetThucInput { get; set; }
    [BindProperty] public int IdLichVoHieu { get; set; }
    [BindProperty] public int SoNgaySinh { get; set; } = 7;

    public async Task OnGetAsync(int? idBacSi = null)
    {
        IdBacSiDangChon = idBacSi;
        await TaiDuLieuAsync(idBacSi);
    }

    public async Task<IActionResult> OnPostTaoMauAsync()
    {
        try
        {
            await _mediator.Send(new TaoLichNoiTruCommand(
                IdBacSiInput,
                IdPhongInput,
                IdDinhNghiaCaInput,
                NgayTrongTuanInput,
                NgayApDungInput,
                NgayKetThucInput));
            TempData["SuccessMessage"] = "Đã tạo mẫu lịch nội trú.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToPage(new { idBacSi = IdBacSiInput });
    }

    public async Task<IActionResult> OnPostVoHieuAsync(int? idBacSi = null)
    {
        try
        {
            await _mediator.Send(new VoHieuLichNoiTruCommand(IdLichVoHieu));
            TempData["SuccessMessage"] = "Đã vô hiệu mẫu lịch.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToPage(new { idBacSi });
    }

    public async Task<IActionResult> OnPostSinhCaAsync(int? idBacSi = null)
    {
        try
        {
            var soCa = await _mediator.Send(new SinhCaLamViecTuLichNoiTruCommand(SoNgaySinh));
            TempData["SuccessMessage"] = $"Đã sinh {soCa} ca làm việc cho {SoNgaySinh} ngày tới.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToPage(new { idBacSi });
    }

    private async Task TaiDuLieuAsync(int? idBacSi)
    {
        DanhSachBacSi = await _mediator.Send(new DanhSachBacSiCongKhaiQuery(
            SoTrang: 1,
            KichThuocTrang: 200,
            DangLamViec: true,
            LoaiHopDong: LoaiHopDong.NoiTru));

        DanhSachPhong = await _mediator.Send(new DanhSachPhongQuery(1, 200, true, null));
        DanhSachDinhNghiaCa = await _mediator.Send(new DanhSachDinhNghiaCaQuery(1, 50, true, null));

        if (idBacSi.HasValue)
        {
            BacSiDangChon = DanhSachBacSi.FirstOrDefault(x => x.IdBacSi == idBacSi.Value);
            DanhSachLich = await _mediator.Send(new DanhSachLichNoiTruTheoBacSiQuery(idBacSi.Value, true));
        }
        else if (DanhSachBacSi.Count > 0)
        {
            // Mac dinh chon BS dau tien
            IdBacSiDangChon = DanhSachBacSi[0].IdBacSi;
            BacSiDangChon = DanhSachBacSi[0];
            DanhSachLich = await _mediator.Send(new DanhSachLichNoiTruTheoBacSiQuery(IdBacSiDangChon.Value, true));
        }
    }
}
