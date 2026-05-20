using ClinicBooking.Application.Features.NghiPhep.Queries.DanhSachDonNghiPhepChoDuyet;
using ClinicBooking.Application.Features.Scheduling.Queries.DanhSachCaLamViecChoDuyet;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ClinicBooking.Web.ViewComponents;

public sealed class AdminBadgeCounterViewComponent : ViewComponent
{
    private readonly IMediator _mediator;

    public AdminBadgeCounterViewComponent(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var soCaChoDuyet = (await _mediator.Send(new DanhSachCaLamViecChoDuyetQuery(1, 1))).Count;
        var soDonNghiPhepChoDuyet = (await _mediator.Send(new DanhSachDonNghiPhepChoDuyetQuery())).Count;
        return View(new AdminBadgeCountersDto(soCaChoDuyet, soDonNghiPhepChoDuyet));
    }

    public sealed record AdminBadgeCountersDto(int SoCaChoDuyet, int SoDonNghiPhepChoDuyet);
}
