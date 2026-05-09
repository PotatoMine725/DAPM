using ClinicBooking.Application.Common;
using ClinicBooking.Application.Features.BenhNhan.Commands.TaoBenhNhanWalkIn;
using ClinicBooking.Application.Features.BenhNhan.Queries.TimBenhNhanTheoSdt;
using ClinicBooking.Application.Features.DanhMuc.Dtos;
using ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachDichVu;
using ClinicBooking.Application.Features.LichHen.Commands.CheckInLichHen;
using ClinicBooking.Application.Features.LichHen.Commands.GanBacSiChoLichHen;
using ClinicBooking.Application.Features.LichHen.Commands.HuyLichHen;
using ClinicBooking.Application.Features.LichHen.Commands.TaoLichHen;
using ClinicBooking.Application.Features.LichHen.Commands.XacNhanLichHen;
using ClinicBooking.Application.Features.LichHen.Dtos;
using ClinicBooking.Application.Features.LichHen.Queries.DanhSachTatCaLichHen;
using ClinicBooking.Application.Features.LichHen.Queries.LayChiTietLichHenLeTan;
using ClinicBooking.Application.Features.LichHen.Queries.TimBacSiKhaDung;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicBooking.Web.Pages.LeTan;

[Authorize(Roles = "le_tan,admin")]
public class QuanLyLichHenModel : PageModel
{
    private readonly IMediator _mediator;
    private const int SoTrenMoiTrang = 10;

    public QuanLyLichHenModel(IMediator mediator) => _mediator = mediator;

    public PhanTrangKetQua<LichHenTomTatResponse> KetQua { get; private set; } = null!;
    public DateOnly? NgayLoc { get; private set; }
    public TrangThaiLichHen? TrangThaiLoc { get; private set; }
    public int TrangHienTai { get; private set; }
    public IReadOnlyList<DichVuResponse> DanhSachDichVu { get; private set; } = [];

    [BindProperty]
    public DatLichVangLaiInput VangLai { get; set; } = new();

    public class DatLichVangLaiInput
    {
        public int? IdBenhNhanCu { get; set; }
        public string? HoTen { get; set; }
        public string SoDienThoai { get; set; } = string.Empty;
        public DateOnly? NgaySinh { get; set; }
        public GioiTinh? GioiTinh { get; set; }
        public string? Cccd { get; set; }
        public DateOnly NgayLamViec { get; set; }
        public TimeOnly GioMongMuon { get; set; }
        public int IdDichVu { get; set; }
        public string? TrieuChung { get; set; }
    }

    public async Task OnGetAsync(DateOnly? ngay, TrangThaiLichHen? trangThai, int trang = 1)
    {
        NgayLoc = ngay;
        TrangThaiLoc = trangThai;
        TrangHienTai = trang;

        KetQua = await _mediator.Send(
            new DanhSachTatCaLichHenQuery(ngay, trangThai, trang, SoTrenMoiTrang));
        DanhSachDichVu = await _mediator.Send(
            new DanhSachDichVuQuery(1, 100, null, true, null));
    }

    public async Task<IActionResult> OnPostDatLichVangLaiAsync()
    {
        try
        {
            int idBenhNhan;
            if (VangLai.IdBenhNhanCu.HasValue)
            {
                idBenhNhan = VangLai.IdBenhNhanCu.Value;
            }
            else
            {
                var ketQuaTao = await _mediator.Send(new TaoBenhNhanWalkInCommand(
                    VangLai.HoTen!, VangLai.SoDienThoai, VangLai.NgaySinh,
                    VangLai.GioiTinh, VangLai.Cccd, null));
                idBenhNhan = ketQuaTao.IdBenhNhan;
            }

            await _mediator.Send(new TaoLichHenCommand(
                VangLai.NgayLamViec, VangLai.GioMongMuon, VangLai.IdDichVu,
                idBenhNhan, null, null, VangLai.TrieuChung));

            TempData["SuccessMessage"] = "Đã đặt lịch hẹn thành công.";
        }
        catch (Exception ex) { TempData["ErrorMessage"] = ex.Message; }
        return RedirectToPage(new { ngay = Request.Query["ngay"], trangThai = Request.Query["trangThai"], trang = Request.Query["trang"] });
    }

    public async Task<IActionResult> OnGetTimBenhNhanAsync(string sdt)
    {
        try
        {
            var ketQua = await _mediator.Send(new TimBenhNhanTheoSdtQuery(sdt));
            return new JsonResult(new { found = ketQua != null, data = ketQua });
        }
        catch (Exception ex)
        {
            return new JsonResult(new { found = false, error = ex.Message });
        }
    }

    public async Task<IActionResult> OnGetChiTietLichHenAsync(int id)
    {
        try
        {
            var ketQua = await _mediator.Send(new LayChiTietLichHenLeTanQuery(id));
            return new JsonResult(ketQua);
        }
        catch (Exception ex)
        {
            return new JsonResult(new { error = ex.Message }) { StatusCode = 400 };
        }
    }

    public async Task<IActionResult> OnGetTimBacSiKhaDungAsync(int idLichHen, string tenBacSi)
    {
        var ds = await _mediator.Send(new TimBacSiKhaDungChoLichHenQuery(idLichHen, tenBacSi ?? ""));
        return new JsonResult(ds);
    }

    public async Task<IActionResult> OnPostGanBacSiAsync(int idLichHen, int idCaLamViecMoi)
    {
        var ketQua = await _mediator.Send(new GanBacSiChoLichHenCommand(idLichHen, idCaLamViecMoi));
        return new JsonResult(ketQua);
    }

    public async Task<IActionResult> OnPostXacNhanAsync(int idLichHen)
    {
        try
        {
            await _mediator.Send(new XacNhanLichHenCommand(idLichHen));
            TempData["SuccessMessage"] = "Đã xác nhận lịch hẹn.";
        }
        catch (Exception ex) { TempData["ErrorMessage"] = ex.Message; }
        return RedirectToPage(new { ngay = Request.Query["ngay"], trangThai = Request.Query["trangThai"], trang = Request.Query["trang"] });
    }

    public async Task<IActionResult> OnPostCheckInAsync(int idLichHen)
    {
        try
        {
            await _mediator.Send(new CheckInLichHenCommand(idLichHen));
            TempData["SuccessMessage"] = "Check-in thành công.";
        }
        catch (Exception ex) { TempData["ErrorMessage"] = ex.Message; }
        return RedirectToPage(new { ngay = Request.Query["ngay"], trangThai = Request.Query["trangThai"], trang = Request.Query["trang"] });
    }

    public async Task<IActionResult> OnPostHuyAsync(int idLichHen, string lyDo)
    {
        try
        {
            await _mediator.Send(new HuyLichHenCommand(idLichHen, lyDo));
            TempData["SuccessMessage"] = "Đã huỷ lịch hẹn.";
        }
        catch (Exception ex) { TempData["ErrorMessage"] = ex.Message; }
        return RedirectToPage(new { ngay = Request.Query["ngay"], trangThai = Request.Query["trangThai"], trang = Request.Query["trang"] });
    }
}
