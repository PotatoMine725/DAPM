using ClinicBooking.Application.Features.LichHen.Dtos;
using ClinicBooking.Application.Features.LichHen.Queries.DanhSachLichHenTheoNgayCuaToi;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicBooking.Web.Pages.BacSi;

[Authorize(Roles = "bac_si")]
public class DanhSachLichHenModel : PageModel
{
    private readonly IMediator _mediator;

    public DanhSachLichHenModel(IMediator mediator) => _mediator = mediator;

    public IReadOnlyList<LichHenTomTatResponse> DanhSach { get; private set; } = [];
    public DateOnly Ngay { get; private set; }
    public TrangThaiLichHen? TrangThai { get; private set; }

    public IReadOnlyList<LichHenTomTatResponse> DanhSachLoc =>
        TrangThai.HasValue
            ? DanhSach.Where(x => x.TrangThai == TrangThai.Value).ToList()
            : DanhSach;

    public async Task OnGetAsync(DateOnly? ngay = null, TrangThaiLichHen? trangThai = null)
    {
        Ngay = ngay ?? DateOnly.FromDateTime(DateTime.UtcNow);
        TrangThai = trangThai;
        DanhSach = await _mediator.Send(new DanhSachLichHenTheoNgayCuaToiQuery(Ngay));
    }
}
