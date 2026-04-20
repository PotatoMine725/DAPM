using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Features.DanhMuc.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachPhong;

public sealed class DanhSachPhongHandler : IRequestHandler<DanhSachPhongQuery, IReadOnlyList<PhongResponse>>
{
    private readonly IAppDbContext _db;

    public DanhSachPhongHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<PhongResponse>> Handle(DanhSachPhongQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Phong.AsNoTracking().AsQueryable();

        if (request.TrangThai.HasValue)
        {
            query = query.Where(x => x.TrangThai == request.TrangThai.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.TuKhoa))
        {
            query = query.Where(x => x.MaPhong.Contains(request.TuKhoa) || x.TenPhong.Contains(request.TuKhoa));
        }

        return await query
            .OrderBy(x => x.MaPhong)
            .Skip((request.SoTrang - 1) * request.KichThuocTrang)
            .Take(request.KichThuocTrang)
            .Select(x => new PhongResponse(
                x.IdPhong,
                x.MaPhong,
                x.TenPhong,
                x.SucChua,
                x.TrangBi,
                x.TrangThai))
            .ToListAsync(cancellationToken);
    }
}
