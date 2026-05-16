using ClinicBooking.Application.Features.DanhMuc.Dtos;
using ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachChuyenKhoaCongKhai;
using ClinicBooking.Application.Features.Doctors.Dtos;
using ClinicBooking.Application.Features.Doctors.Queries.DanhSachBacSiCongKhai;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicBooking.Web.Pages.BacSi;

[AllowAnonymous]
public class DanhSachModel : PageModel
{
    private readonly IMediator _mediator;

    public DanhSachModel(IMediator mediator) => _mediator = mediator;

    public IReadOnlyList<BacSiPublicResponse> DanhSachBacSi { get; private set; } = [];
    public IReadOnlyList<ChuyenKhoaCongKhaiResponse> DanhSachChuyenKhoa { get; private set; } = [];
    public int? IdChuyenKhoa { get; private set; }
    public string? TuKhoa { get; private set; }

    public async Task OnGetAsync(int? idChuyenKhoa = null, string? tuKhoa = null)
    {
        IdChuyenKhoa = idChuyenKhoa;
        TuKhoa = tuKhoa;
        DanhSachChuyenKhoa = await _mediator.Send(new DanhSachChuyenKhoaCongKhaiQuery(1, 200, null));
        DanhSachBacSi = await _mediator.Send(new DanhSachBacSiCongKhaiQuery(
            SoTrang: 1,
            KichThuocTrang: 100,
            IdChuyenKhoa: idChuyenKhoa,
            TuKhoa: tuKhoa,
            DangLamViec: true));
    }
}
