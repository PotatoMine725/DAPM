using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.NghiPhep.Queries.DanhSachDonNghiPhep;

public sealed class DanhSachDonNghiPhepHandler : IRequestHandler<DanhSachDonNghiPhepQuery, IReadOnlyList<object>>
{
    private readonly IAppDbContext _db;

    public DanhSachDonNghiPhepHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<object>> Handle(DanhSachDonNghiPhepQuery request, CancellationToken cancellationToken)
    {
        var query = _db.DonNghiPhep.AsNoTracking().AsQueryable();
        if (request.TrangThaiDuyet.HasValue)
        {
            query = query.Where(x => x.TrangThaiDuyet == request.TrangThaiDuyet.Value);
        }

        return await query.Select(x => new { x.IdDonNghiPhep }).ToListAsync(cancellationToken).ContinueWith(t => (IReadOnlyList<object>)t.Result, cancellationToken);
    }
}
