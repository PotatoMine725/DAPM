using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Features.Scheduling.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.Scheduling.Queries.DanhSachLichNoiTruTheoBacSi;

public sealed class DanhSachLichNoiTruTheoBacSiHandler
    : IRequestHandler<DanhSachLichNoiTruTheoBacSiQuery, IReadOnlyList<LichNoiTruResponse>>
{
    private readonly IAppDbContext _db;

    public DanhSachLichNoiTruTheoBacSiHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<LichNoiTruResponse>> Handle(
        DanhSachLichNoiTruTheoBacSiQuery request,
        CancellationToken cancellationToken)
    {
        var query = _db.LichNoiTru
            .AsNoTracking()
            .Include(x => x.BacSi)
            .Include(x => x.Phong)
            .Include(x => x.DinhNghiaCa)
            .AsQueryable();

        if (request.IdBacSi.HasValue)
        {
            query = query.Where(x => x.IdBacSi == request.IdBacSi.Value);
        }

        if (request.ChiHienHieuLuc == true)
        {
            query = query.Where(x => x.TrangThai);
        }

        return await query
            .OrderBy(x => x.IdBacSi)
            .ThenBy(x => x.NgayTrongTuan)
            .ThenBy(x => x.DinhNghiaCa.GioBatDauMacDinh)
            .Select(x => LichNoiTruResponse.TuEntity(x))
            .ToListAsync(cancellationToken);
    }
}
