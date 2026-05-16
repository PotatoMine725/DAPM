using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.Scheduling.Queries.KiemTraDoPhuBacSi;

public sealed class KiemTraDoPhuBacSiHandler : IRequestHandler<KiemTraDoPhuBacSiQuery, KiemTraDoPhuBacSiResponse>
{
    private readonly IAppDbContext _db;

    public KiemTraDoPhuBacSiHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<KiemTraDoPhuBacSiResponse> Handle(KiemTraDoPhuBacSiQuery request, CancellationToken cancellationToken)
    {
        var ds = await _db.CaLamViec
            .AsNoTracking()
            .Where(x => x.IdChuyenKhoa == request.IdChuyenKhoa)
            .Where(x => x.NgayLamViec >= request.TuNgay && x.NgayLamViec <= request.DenNgay)
            .GroupBy(x => x.NgayLamViec)
            .Select(g => new
            {
                Ngay = g.Key,
                SoCaDaDuyet = g.Count(x => x.TrangThaiDuyet == TrangThaiDuyetCa.DaDuyet),
                SoCaChoDuyet = g.Count(x => x.TrangThaiDuyet == TrangThaiDuyetCa.ChoDuyet)
            })
            .Where(x => x.SoCaDaDuyet == 0)
            .Select(x => new NgayThieuBacSiDto(x.Ngay, x.SoCaChoDuyet, x.SoCaDaDuyet == 0 && x.SoCaChoDuyet == 0))
            .OrderBy(x => x.Ngay)
            .ToListAsync(cancellationToken);

        return new KiemTraDoPhuBacSiResponse(ds);
    }
}