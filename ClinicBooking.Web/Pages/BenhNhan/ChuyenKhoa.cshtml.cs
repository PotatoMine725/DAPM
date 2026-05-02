using ClinicBooking.Application.Features.Doctors.Dtos;
using ClinicBooking.Application.Features.Doctors.Queries.DanhSachBacSiCongKhai;
using ClinicBooking.Application.Features.DanhMuc.Dtos;
using ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachChuyenKhoaCongKhai;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicBooking.Web.Pages.BenhNhan;

[Authorize(Roles = "benh_nhan")]
public class ChuyenKhoaModel : PageModel
{
    private readonly IMediator _mediator;

    public ChuyenKhoaModel(IMediator mediator) => _mediator = mediator;

    public IReadOnlyList<ChuyenKhoaCongKhaiResponse> DanhSachChuyenKhoa { get; private set; } = [];
    public IReadOnlyList<BacSiPublicResponse> DanhSachBacSi { get; private set; } = [];
    public int? IdChuyenKhoa { get; private set; }

    public async Task OnGetAsync(int? idChuyenKhoa = null)
    {
        IdChuyenKhoa = idChuyenKhoa;
        DanhSachChuyenKhoa = await _mediator.Send(new DanhSachChuyenKhoaCongKhaiQuery());
        DanhSachBacSi = await _mediator.Send(new DanhSachBacSiCongKhaiQuery(
            SoTrang: 1,
            KichThuocTrang: 100,
            IdChuyenKhoa: idChuyenKhoa,
            DangLamViec: true));
    }
}
