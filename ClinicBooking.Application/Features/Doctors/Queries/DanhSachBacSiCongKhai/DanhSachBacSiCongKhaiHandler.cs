using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Features.Doctors.Dtos;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.Doctors.Queries.DanhSachBacSiCongKhai;

public sealed class DanhSachBacSiCongKhaiHandler : IRequestHandler<DanhSachBacSiCongKhaiQuery, IReadOnlyList<BacSiPublicResponse>>
{
    private readonly IAppDbContext _db;

    public DanhSachBacSiCongKhaiHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<BacSiPublicResponse>> Handle(DanhSachBacSiCongKhaiQuery request, CancellationToken cancellationToken)
    {
        var query = _db.BacSi
            .AsNoTracking()
            .Include(x => x.ChuyenKhoa)
            .AsQueryable();

        query = query.Where(x => x.TrangThai == TrangThaiBacSi.DangLam && x.ChuyenKhoa.HienThi);

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
            .Select(x => BacSiPublicResponse.TuEntity(x))
            .ToListAsync(cancellationToken);
    }
}
