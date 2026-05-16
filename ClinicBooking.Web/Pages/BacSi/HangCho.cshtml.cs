using ClinicBooking.Application.Features.Doctors.Queries.LayHoSoBacSiCuaToi;
using ClinicBooking.Application.Features.Doctors.Dtos;
using ClinicBooking.Application.Features.HangCho.Commands.GoiBenhNhanKeTiep;
using ClinicBooking.Application.Features.HangCho.Commands.HoanThanhLuotKham;
using ClinicBooking.Application.Features.HangCho.Dtos;
using ClinicBooking.Application.Features.HangCho.Queries.XemHangChoTheoCa;
using ClinicBooking.Application.Features.Scheduling.Dtos;
using ClinicBooking.Application.Features.Scheduling.Queries.DanhSachCaLamViecCongKhai;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicBooking.Web.Pages.BacSi;

[Authorize(Roles = "bac_si")]
public class HangChoModel : PageModel
{
    private readonly IMediator _mediator;

    public HangChoModel(IMediator mediator) => _mediator = mediator;

    public HoSoBacSiCuaToiResponse? HoSo { get; private set; }
    public IReadOnlyList<CaLamViecPublicResponse> DanhSachCa { get; private set; } = [];
    public Dictionary<int, IReadOnlyList<HangChoResponse>> HangChoTheoCA { get; private set; } = new();
    public int? IdCaLamViec { get; private set; }
    public bool LaAutoChon { get; private set; }

    public IReadOnlyList<HangChoResponse> HangCho =>
        IdCaLamViec.HasValue && HangChoTheoCA.TryGetValue(IdCaLamViec.Value, out var ds) ? ds : [];

    public int SoChoKham   => HangCho.Count(x => x.TrangThai == TrangThaiHangCho.ChoKham);
    public int SoDangKham  => HangCho.Count(x => x.TrangThai == TrangThaiHangCho.DangKham);
    public int SoHoanThanh => HangCho.Count(x => x.TrangThai == TrangThaiHangCho.HoanThanh);

    public int SoChoKhamTheoCA(int idCa) =>
        HangChoTheoCA.TryGetValue(idCa, out var ds) ? ds.Count(x => x.TrangThai == TrangThaiHangCho.ChoKham) : 0;
    public int SoDangKhamTheoCA(int idCa) =>
        HangChoTheoCA.TryGetValue(idCa, out var ds) ? ds.Count(x => x.TrangThai == TrangThaiHangCho.DangKham) : 0;

    public async Task OnGetAsync(int? idCaLamViec)
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        HoSo = await _mediator.Send(new LayHoSoBacSiCuaToiQuery());

        if (HoSo is null) return;

        DanhSachCa = await _mediator.Send(new DanhSachCaLamViecCongKhaiQuery(
            TuNgay: today,
            DenNgay: today,
            IdBacSi: HoSo.IdBacSi,
            KichThuocTrang: 50));

        // Load queue tuần tự — EF Core DbContext không thread-safe, không dùng Task.WhenAll
        foreach (var ca in DanhSachCa)
        {
            var ds = await _mediator.Send(new XemHangChoTheoCaQuery(ca.IdCaLamViec));
            HangChoTheoCA[ca.IdCaLamViec] = ds;
        }

        // Auth check: param phải thuộc ca của bác sĩ này
        var idCaHopLe = idCaLamViec.HasValue && DanhSachCa.Any(c => c.IdCaLamViec == idCaLamViec.Value)
            ? idCaLamViec
            : null;

        if (idCaHopLe.HasValue)
        {
            IdCaLamViec = idCaHopLe;
            LaAutoChon = false;
        }
        else
        {
            IdCaLamViec = AutoChonCa();
            LaAutoChon = IdCaLamViec.HasValue;
        }
    }

    public async Task<IActionResult> OnPostGoiKeTiepAsync(int idCaLamViec)
    {
        try
        {
            await _mediator.Send(new GoiBenhNhanKeTiepCommand(idCaLamViec));
            TempData["SuccessMessage"] = "Đã gọi bệnh nhân tiếp theo.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToPage(new { idCaLamViec });
    }

    public async Task<IActionResult> OnPostHoanThanhAsync(int idHangCho, int idCaLamViec)
    {
        try
        {
            await _mediator.Send(new HoanThanhLuotKhamCommand(idHangCho));
            TempData["SuccessMessage"] = "Đã hoàn thành lượt khám.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToPage(new { idCaLamViec });
    }

    // 4-tier auto-select:
    // 1. Ca đang trong giờ + có ChoKham/DangKham
    // 2. Bất kỳ ca nào có ChoKham/DangKham (bác sĩ khám trễ/sớm)
    // 3. Ca đầu tiên hôm nay
    // 4. null
    private int? AutoChonCa()
    {
        if (!DanhSachCa.Any()) return null;

        var now = TimeOnly.FromDateTime(DateTime.Now);

        var caDangTrong = DanhSachCa.FirstOrDefault(ca =>
            ca.GioBatDau <= now && now <= ca.GioKetThuc &&
            (SoChoKhamTheoCA(ca.IdCaLamViec) > 0 || SoDangKhamTheoCA(ca.IdCaLamViec) > 0));
        if (caDangTrong is not null) return caDangTrong.IdCaLamViec;

        var caCoBenhNhan = DanhSachCa.FirstOrDefault(ca =>
            SoChoKhamTheoCA(ca.IdCaLamViec) > 0 || SoDangKhamTheoCA(ca.IdCaLamViec) > 0);
        if (caCoBenhNhan is not null) return caCoBenhNhan.IdCaLamViec;

        return DanhSachCa[0].IdCaLamViec;
    }
}
