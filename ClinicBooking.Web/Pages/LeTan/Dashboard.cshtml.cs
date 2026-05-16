using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Features.LichHen.Commands.CheckInLichHen;
using ClinicBooking.Application.Features.LichHen.Commands.XacNhanLichHen;
using ClinicBooking.Application.Features.LichHen.Dtos;
using ClinicBooking.Application.Features.LichHen.Queries.DanhSachLichHenTheoNgay;
using ClinicBooking.Application.Features.DanhMuc.Dtos;
using ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachChuyenKhoa;
using ClinicBooking.Application.Features.Scheduling.Queries.KiemTraDoPhuBacSi;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicBooking.Web.Pages.LeTan;

[Authorize(Roles = "le_tan,admin")]
public class DashboardModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly IDateTimeProvider _dateTimeProvider;

    public DashboardModel(IMediator mediator, IDateTimeProvider dateTimeProvider)
    {
        _mediator = mediator;
        _dateTimeProvider = dateTimeProvider;
    }

    public sealed record CanhBaoChuyenKhoaVm(int IdChuyenKhoa, string TenChuyenKhoa, IReadOnlyList<NgayThieuBacSiDto> NgayThieu);

    public IReadOnlyList<LichHenTomTatResponse> LichHenHomNay { get; private set; } = [];
    public IReadOnlyList<ChuyenKhoaResponse> DanhSachChuyenKhoa { get; private set; } = [];
    public IReadOnlyList<CanhBaoChuyenKhoaVm> CanhBaoTheoChuyenKhoa { get; private set; } = [];
    public int TongLichHen => LichHenHomNay.Count;
    public int SoChoXacNhan => LichHenHomNay.Count(x => x.TrangThai == TrangThaiLichHen.ChoXacNhan);
    public int SoDaXacNhan => LichHenHomNay.Count(x => x.TrangThai == TrangThaiLichHen.DaXacNhan);
    public DateOnly NgayHienTai { get; private set; }

    public async Task OnGetAsync()
    {
        NgayHienTai = DateOnly.FromDateTime(_dateTimeProvider.UtcNow);
        LichHenHomNay = await _mediator.Send(new DanhSachLichHenTheoNgayQuery(NgayHienTai));
        DanhSachChuyenKhoa = await _mediator.Send(new DanhSachChuyenKhoaQuery(1, 100, true, null));
        CanhBaoTheoChuyenKhoa = DanhSachChuyenKhoa.Count == 0
            ? []
            : (await Task.WhenAll(DanhSachChuyenKhoa.Select(async ck => new CanhBaoChuyenKhoaVm(
                ck.IdChuyenKhoa,
                ck.TenChuyenKhoa,
                (await _mediator.Send(new KiemTraDoPhuBacSiQuery(ck.IdChuyenKhoa, NgayHienTai, NgayHienTai.AddDays(7)))).NgayThieu)))).ToList();
    }

    public async Task<IActionResult> OnPostXacNhanAsync(int idLichHen)
    {
        try
        {
            await _mediator.Send(new XacNhanLichHenCommand(idLichHen));
            TempData["SuccessMessage"] = "Đã xác nhận lịch hẹn.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostCheckInAsync(int idLichHen)
    {
        try
        {
            await _mediator.Send(new CheckInLichHenCommand(idLichHen));
            TempData["SuccessMessage"] = "Check-in thành công. Bệnh nhân đã vào hàng chờ.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToPage();
    }
}