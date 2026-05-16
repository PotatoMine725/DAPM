using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.Scheduling.Queries.ThongKeDuyetCa;

public sealed class ThongKeDuyetCaHandler
    : IRequestHandler<ThongKeDuyetCaQuery, ThongKeDuyetCaResponse>
{
    private readonly IAppDbContext _db;

    public ThongKeDuyetCaHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<ThongKeDuyetCaResponse> Handle(
        ThongKeDuyetCaQuery request,
        CancellationToken cancellationToken)
    {
        var caQuery = _db.CaLamViec.AsNoTracking().AsQueryable();

        if (request.TuNgay.HasValue)
        {
            caQuery = caQuery.Where(x => x.NgayLamViec >= request.TuNgay.Value);
        }
        if (request.DenNgay.HasValue)
        {
            caQuery = caQuery.Where(x => x.NgayLamViec <= request.DenNgay.Value);
        }

        var soChoDuyet = await caQuery
            .CountAsync(x => x.TrangThaiDuyet == TrangThaiDuyetCa.ChoDuyet, cancellationToken);
        var soDaDuyet = await caQuery
            .CountAsync(x => x.TrangThaiDuyet == TrangThaiDuyetCa.DaDuyet, cancellationToken);
        var soTuChoi = await caQuery
            .CountAsync(x => x.TrangThaiDuyet == TrangThaiDuyetCa.DaHuy, cancellationToken);

        var soBacSiHopDong = await _db.BacSi
            .AsNoTracking()
            .CountAsync(x => x.LoaiHopDong == LoaiHopDong.HopDong
                          && x.TrangThai == TrangThaiBacSi.DangLam, cancellationToken);

        return new ThongKeDuyetCaResponse(soChoDuyet, soDaDuyet, soTuChoi, soBacSiHopDong);
    }
}
