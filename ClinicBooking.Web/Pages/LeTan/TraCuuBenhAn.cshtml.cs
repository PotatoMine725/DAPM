using ClinicBooking.Application.Features.BenhNhan.Dtos;
using ClinicBooking.Application.Features.BenhNhan.Queries.LayBenhNhanById;
using ClinicBooking.Application.Features.BenhNhan.Queries.TimKiemBenhNhan;
using ClinicBooking.Application.Features.HoSoKham.Dtos;
using ClinicBooking.Application.Features.HoSoKham.Queries.LichSuKhamTheoBenhNhan;
using ClinicBooking.Application.Features.ToaThuoc.Dtos;
using ClinicBooking.Application.Features.ToaThuoc.Queries.LichSuToaThuocTheoBenhNhan;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicBooking.Web.Pages.LeTan;

[Authorize(Roles = "le_tan,admin")]
public class TraCuuBenhAnModel : PageModel
{
    private readonly IMediator _mediator;

    public TraCuuBenhAnModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    [BindProperty(SupportsGet = true)]
    public string? TuKhoa { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? IdBenhNhan { get; set; }

    public IReadOnlyList<BenhNhanResponse> KetQuaTimKiem { get; private set; } = [];
    public BenhNhanResponse? BenhNhanDuocChon { get; private set; }
    public IReadOnlyList<HoSoKhamTomTatResponse> LichSuKham { get; private set; } = [];
    public IReadOnlyList<ToaThuocResponse> ToaGanDay { get; private set; } = [];

    public async Task OnGetAsync()
    {
        if (!string.IsNullOrWhiteSpace(TuKhoa))
        {
            KetQuaTimKiem = await _mediator.Send(new TimKiemBenhNhanQuery(TuKhoa, 20));
        }

        if (IdBenhNhan.HasValue)
        {
            BenhNhanDuocChon = await _mediator.Send(new LayBenhNhanByIdQuery(IdBenhNhan.Value));
            LichSuKham = await _mediator.Send(new LichSuKhamTheoBenhNhanQuery(IdBenhNhan.Value, 1, 20));
            ToaGanDay = await _mediator.Send(new LichSuToaThuocTheoBenhNhanQuery(IdBenhNhan.Value, 1, 10));
        }
    }
}
