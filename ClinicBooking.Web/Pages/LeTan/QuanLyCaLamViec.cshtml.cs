using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Features.DanhMuc.Dtos;
using ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachChuyenKhoa;
using ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachPhong;
using ClinicBooking.Application.Features.Scheduling.Commands.DuyetCaLamViec;
using ClinicBooking.Application.Features.Scheduling.Commands.XoaCaLamViec;
using ClinicBooking.Application.Features.Scheduling.Dtos;
using ClinicBooking.Application.Features.Scheduling.Queries.DanhSachCaLamViecCongKhai;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicBooking.Web.Pages.LeTan;

[Authorize(Roles = "le_tan,admin")]
public class QuanLyCaLamViecModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public QuanLyCaLamViecModel(IMediator mediator, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    public IReadOnlyList<CaLamViecPublicResponse> DanhSach { get; private set; } = [];
    public IReadOnlyList<ChuyenKhoaResponse> DanhSachChuyenKhoa { get; private set; } = [];
    public IReadOnlyList<PhongResponse> DanhSachPhong { get; private set; } = [];

    public int TongSoCa => DanhSach.Count;
    public int SoCaDaDuyet => DanhSach.Count(x => x.TrangThaiDuyet == TrangThaiDuyetCa.DaDuyet.ToString());
    public int SoCaChoDuyet => DanhSach.Count(x => x.TrangThaiDuyet == TrangThaiDuyetCa.ChoDuyet.ToString());
    public int SoCaConTrong => DanhSach.Count(x => x.ConTrong);

    public int? IdChuyenKhoaLoc { get; private set; }
    public int? IdPhongLoc { get; private set; }
    public DateOnly? TuNgayLoc { get; private set; }
    public DateOnly? DenNgayLoc { get; private set; }
    public bool? ConTrongLoc { get; private set; }

    [BindProperty] public int IdCaLamViec { get; set; }
    [BindProperty] public bool ChapNhan { get; set; } = true;
    [BindProperty] public string? LyDoTuChoi { get; set; }

    public async Task OnGetAsync(int? idChuyenKhoa = null, int? idPhong = null, DateOnly? tuNgay = null, DateOnly? denNgay = null, bool? conTrong = null)
    {
        IdChuyenKhoaLoc = idChuyenKhoa;
        IdPhongLoc = idPhong;
        TuNgayLoc = tuNgay;
        DenNgayLoc = denNgay;
        ConTrongLoc = conTrong;

        await TaiDuLieuAsync(idChuyenKhoa, idPhong, tuNgay, denNgay, conTrong);
    }

    public async Task<IActionResult> OnPostDuyetAsync(int idCaLamViec, bool chapNhan, string? lyDoTuChoi, int? idChuyenKhoa = null, int? idPhong = null, DateOnly? tuNgay = null, DateOnly? denNgay = null, bool? conTrong = null)
    {
        try
        {
            var idNguoiDuyet = _currentUser.IdTaiKhoan ?? 0;
            await _mediator.Send(new DuyetCaLamViecCommand(idCaLamViec, chapNhan, lyDoTuChoi, idNguoiDuyet));
            TempData["SuccessMessage"] = chapNhan ? "Đã duyệt ca làm việc." : "Đã từ chối ca làm việc.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToPage(new { idChuyenKhoa, idPhong, tuNgay, denNgay, conTrong });
    }

    public async Task<IActionResult> OnPostXoaAsync(int idCaLamViec, int? idChuyenKhoa = null, int? idPhong = null, DateOnly? tuNgay = null, DateOnly? denNgay = null, bool? conTrong = null)
    {
        try
        {
            await _mediator.Send(new XoaCaLamViecCommand(idCaLamViec));
            TempData["SuccessMessage"] = "Đã xóa ca làm việc.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToPage(new { idChuyenKhoa, idPhong, tuNgay, denNgay, conTrong });
    }

    private async Task TaiDuLieuAsync(int? idChuyenKhoa, int? idPhong, DateOnly? tuNgay, DateOnly? denNgay, bool? conTrong)
    {
        DanhSachChuyenKhoa = await _mediator.Send(new DanhSachChuyenKhoaQuery(1, 200, true, null));
        DanhSachPhong = await _mediator.Send(new DanhSachPhongQuery(1, 200, true, null));
        DanhSach = await _mediator.Send(new DanhSachCaLamViecCongKhaiQuery(
            SoTrang: 1,
            KichThuocTrang: 200,
            IdChuyenKhoa: idChuyenKhoa,
            IdPhong: idPhong,
            TuNgay: tuNgay,
            DenNgay: denNgay,
            ConTrong: conTrong));
    }
}
