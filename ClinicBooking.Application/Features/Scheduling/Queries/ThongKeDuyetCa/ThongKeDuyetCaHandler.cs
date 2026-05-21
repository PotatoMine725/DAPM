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

        // Run all count queries in parallel for better performance
        var soChoDuyetTask = query.CountAsync(x => x.TrangThaiDuyet == TrangThaiDuyetCa.ChoDuyet, cancellationToken);
        var soDaDuyetTask = query.CountAsync(x => x.TrangThaiDuyet == TrangThaiDuyetCa.DaDuyet, cancellationToken);
        var soTuChoiTask = query.CountAsync(x => x.TrangThaiDuyet == TrangThaiDuyetCa.DaHuy, cancellationToken);
        var soBacSiHopDongTask = _db.BacSi.AsNoTracking().CountAsync(x => x.LoaiHopDong == LoaiHopDong.HopDong, cancellationToken);

        await Task.WhenAll(soChoDuyetTask, soDaDuyetTask, soTuChoiTask, soBacSiHopDongTask);

        return new ThongKeDuyetCaResponse(
            soChoDuyetTask.Result,
            soDaDuyetTask.Result,
            soTuChoiTask.Result,
            soBacSiHopDongTask.Result);
    }
}
