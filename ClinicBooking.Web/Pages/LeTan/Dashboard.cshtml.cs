using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Features.LichHen.Commands.CheckInLichHen;
using ClinicBooking.Application.Features.LichHen.Commands.XacNhanLichHen;
using ClinicBooking.Application.Features.LichHen.Dtos;
using ClinicBooking.Application.Features.LichHen.Queries.DanhSachLichHenTheoNgay;
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

    public IReadOnlyList<LichHenTomTatResponse> LichHenHomNay { get; private set; } = [];
    public int TongLichHen  => LichHenHomNay.Count;
    public int SoChoXacNhan => LichHenHomNay.Count(x => x.TrangThai == TrangThaiLichHen.ChoXacNhan);
    public int SoDaXacNhan  => LichHenHomNay.Count(x => x.TrangThai == TrangThaiLichHen.DaXacNhan);
    public DateOnly NgayHienTai { get; private set; }

    public async Task OnGetAsync()
    {
        NgayHienTai = DateOnly.FromDateTime(_dateTimeProvider.UtcNow);
        LichHenHomNay = await _mediator.Send(new DanhSachLichHenTheoNgayQuery(NgayHienTai));
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
