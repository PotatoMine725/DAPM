using ClinicBooking.Application.Common.Constants;
using ClinicBooking.Application.Features.DanhMuc.Dtos;
using ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachChuyenKhoa;
using ClinicBooking.Application.Features.Scheduling.Dtos;
using ClinicBooking.Application.Features.Scheduling.Queries.KiemTraDoPhuBacSi;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicBooking.Web.Pages.Admin;

[Authorize(Roles = VaiTroConstants.Admin)]
public class DashboardModel : PageModel
{
    private readonly IMediator _mediator;

    public DashboardModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public string? ErrorMessage { get; private set; }
    public sealed record CanhBaoChuyenKhoaVm(int IdChuyenKhoa, string TenChuyenKhoa, IReadOnlyList<NgayThieuBacSiDto> NgayThieu);

    public IReadOnlyList<ChuyenKhoaResponse> DanhSachChuyenKhoa { get; private set; } = [];
    public IReadOnlyList<CanhBaoChuyenKhoaVm> CanhBaoTheoChuyenKhoa { get; private set; } = [];
    public IReadOnlyList<NgayThieuBacSiDto> NgayThieuTongHop { get; private set; } = [];

    public async Task OnGetAsync()
    {
        DanhSachChuyenKhoa = await _mediator.Send(new DanhSachChuyenKhoaQuery(1, 100, true, null));
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var ketQua = DanhSachChuyenKhoa.Count == 0
            ? []
            : await Task.WhenAll(DanhSachChuyenKhoa.Select(async ck => new CanhBaoChuyenKhoaVm(
                ck.IdChuyenKhoa,
                ck.TenChuyenKhoa,
                (await _mediator.Send(new KiemTraDoPhuBacSiQuery(ck.IdChuyenKhoa, today, today.AddDays(7)))).NgayThieu)));

        CanhBaoTheoChuyenKhoa = ketQua.ToList();
        NgayThieuTongHop = CanhBaoTheoChuyenKhoa.SelectMany(x => x.NgayThieu).ToList();
    }
}
