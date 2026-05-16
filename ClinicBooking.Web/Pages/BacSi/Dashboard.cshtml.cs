using ClinicBooking.Application.Features.Doctors.Dtos;
using ClinicBooking.Application.Features.Doctors.Queries.LayHoSoBacSiCuaToi;
using ClinicBooking.Application.Features.LichHen.Dtos;
using ClinicBooking.Application.Features.LichHen.Queries.DanhSachLichHenTheoNgayCuaToi;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicBooking.Web.Pages.BacSi;

[Authorize(Roles = "bac_si")]
public class DashboardModel : PageModel
{
    private readonly IMediator _mediator;

    public DashboardModel(IMediator mediator) => _mediator = mediator;

    public HoSoBacSiCuaToiResponse? HoSo { get; private set; }
    public IReadOnlyList<LichHenTomTatResponse> LichHenHomNay { get; private set; } = [];

    public int TongLichHen    => LichHenHomNay.Count;
    public int SoChoXacNhan   => LichHenHomNay.Count(x => x.TrangThai == TrangThaiLichHen.ChoXacNhan);
    public int SoDaCheckIn    => LichHenHomNay.Count(x => x.DaCheckIn);
    public int SoDangKham     => LichHenHomNay.Count(x => x.TrangThai == TrangThaiLichHen.DangKham);
    public int SoHoanThanh    => LichHenHomNay.Count(x => x.TrangThai == TrangThaiLichHen.HoanThanh);

    public async Task OnGetAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        HoSo = await _mediator.Send(new LayHoSoBacSiCuaToiQuery());
        LichHenHomNay = await _mediator.Send(new DanhSachLichHenTheoNgayCuaToiQuery(today));
    }
}
