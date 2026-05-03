using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Features.BacSi.Dtos;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.BacSi.Queries.DanhSachBacSi;

public sealed class DanhSachBacSiHandler : IRequestHandler<DanhSachBacSiQuery, IReadOnlyList<BacSiResponse>>
{
    private readonly IAppDbContext _db;

    public DanhSachBacSiHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<BacSiResponse>> Handle(DanhSachBacSiQuery request, CancellationToken cancellationToken)
    {
        var query = _db.BacSi
            .AsNoTracking()
            .Include(x => x.ChuyenKhoa)
            .Where(x => x.TrangThai == TrangThaiBacSi.DangLam)
            .AsQueryable();

        if (request.IdChuyenKhoa.HasValue)
        {
            query = query.Where(x => x.IdChuyenKhoa == request.IdChuyenKhoa.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.TuKhoa))
        {
            query = query.Where(x => x.HoTen.Contains(request.TuKhoa));
        }

        return await query
            .OrderBy(x => x.HoTen)
            .Skip((request.SoTrang - 1) * request.KichThuocTrang)
            .Take(request.KichThuocTrang)
            .Select(x => BacSiResponse.TuEntity(x))
            .ToListAsync(cancellationToken);
    }
}
