using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Features.Scheduling.Dtos;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.Scheduling.Queries.DanhSachCaLamViecCongKhai;

public sealed class DanhSachCaLamViecCongKhaiHandler : IRequestHandler<DanhSachCaLamViecCongKhaiQuery, IReadOnlyList<CaLamViecPublicResponse>>
{
    private readonly IAppDbContext _db;

    public DanhSachCaLamViecCongKhaiHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<CaLamViecPublicResponse>> Handle(DanhSachCaLamViecCongKhaiQuery request, CancellationToken cancellationToken)
    {
        var query = _db.CaLamViec
            .AsNoTracking()
            .Include(x => x.BacSi)
            .Include(x => x.Phong)
            .Include(x => x.ChuyenKhoa)
            .AsQueryable();

        if (request.IdBacSi.HasValue)
        {
            query = query.Where(x => x.IdBacSi == request.IdBacSi.Value);
        }

        if (request.IdChuyenKhoa.HasValue)
        {
            query = query.Where(x => x.IdChuyenKhoa == request.IdChuyenKhoa.Value);
        }

        if (request.IdPhong.HasValue)
        {
            query = query.Where(x => x.IdPhong == request.IdPhong.Value);
        }

        if (request.TuNgay.HasValue)
        {
            query = query.Where(x => x.NgayLamViec >= request.TuNgay.Value);
        }

        if (request.DenNgay.HasValue)
        {
            query = query.Where(x => x.NgayLamViec <= request.DenNgay.Value);
        }

        if (request.TrangThaiDuyet.HasValue)
        {
            query = query.Where(x => x.TrangThaiDuyet == request.TrangThaiDuyet.Value);
        }

        if (request.ConTrong.HasValue)
        {
            query = query.Where(x => (x.SoSlotDaDat < x.SoSlotToiDa) == request.ConTrong.Value);
        }

        return await query
            .OrderBy(x => x.NgayLamViec)
            .ThenBy(x => x.GioBatDau)
            .Skip((request.SoTrang - 1) * request.KichThuocTrang)
            .Take(request.KichThuocTrang)
            .Select(x => CaLamViecPublicResponse.TuEntity(x))
            .ToListAsync(cancellationToken);
    }
}
