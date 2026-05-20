using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.Scheduling.Queries.ThongKeDuyetCa;

public sealed class ThongKeDuyetCaHandler : IRequestHandler<ThongKeDuyetCaQuery, ThongKeDuyetCaResponse>
{
    private readonly IAppDbContext _db;

    public ThongKeDuyetCaHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<ThongKeDuyetCaResponse> Handle(ThongKeDuyetCaQuery request, CancellationToken cancellationToken)
    {
        var query = _db.CaLamViec.AsNoTracking()
            .Where(x => x.NgayLamViec >= request.TuNgay && x.NgayLamViec <= request.DenNgay);

        var soChoDuyet = await query.CountAsync(x => x.TrangThaiDuyet == TrangThaiDuyetCa.ChoDuyet, cancellationToken);
        var soDaDuyet = await query.CountAsync(x => x.TrangThaiDuyet == TrangThaiDuyetCa.DaDuyet, cancellationToken);
        var soTuChoi = await query.CountAsync(x => x.TrangThaiDuyet == TrangThaiDuyetCa.DaHuy, cancellationToken);
        var soBacSiHopDong = await _db.BacSi.AsNoTracking().CountAsync(x => x.LoaiHopDong == LoaiHopDong.HopDong, cancellationToken);

        return new ThongKeDuyetCaResponse(soChoDuyet, soDaDuyet, soTuChoi, soBacSiHopDong);
    }
}
