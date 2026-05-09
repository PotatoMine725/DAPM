using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Features.NghiPhep.Dtos;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.NghiPhep.Queries.DanhSachDonNghiPhep;

public sealed class DanhSachDonNghiPhepHandler : IRequestHandler<DanhSachDonNghiPhepQuery, IReadOnlyList<DonNghiPhepResponse>>
{
    private readonly IAppDbContext _db;

    public DanhSachDonNghiPhepHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<DonNghiPhepResponse>> Handle(DanhSachDonNghiPhepQuery request, CancellationToken cancellationToken)
    {
        var query = _db.DonNghiPhep
            .AsNoTracking()
            .Include(x => x.BacSi)
                .ThenInclude(x => x.ChuyenKhoa)
            .Include(x => x.CaLamViec)
            .AsQueryable();

        if (request.TrangThaiDuyet.HasValue)
        {
            query = query.Where(x => x.TrangThaiDuyet == request.TrangThaiDuyet.Value);
        }

        return await query
            .OrderByDescending(x => x.NgayGuiDon)
            .Select(x => DonNghiPhepResponse.TuEntity(x))
            .ToListAsync(cancellationToken);
    }
}
