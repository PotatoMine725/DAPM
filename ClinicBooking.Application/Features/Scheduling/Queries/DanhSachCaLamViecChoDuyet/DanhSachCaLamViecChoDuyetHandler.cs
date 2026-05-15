using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Features.Scheduling.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.Scheduling.Queries.DanhSachCaLamViecChoDuyet;

public sealed class DanhSachCaLamViecChoDuyetHandler
    : IRequestHandler<DanhSachCaLamViecChoDuyetQuery, IReadOnlyList<CaLamViecAdminResponse>>
{
    private readonly IAppDbContext _db;

    public DanhSachCaLamViecChoDuyetHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<CaLamViecAdminResponse>> Handle(
        DanhSachCaLamViecChoDuyetQuery request,
        CancellationToken cancellationToken)
    {
        var query = _db.CaLamViec
            .AsNoTracking()
            .Include(x => x.BacSi)
            .Include(x => x.ChuyenKhoa)
            .Include(x => x.Phong)
            .Include(x => x.DinhNghiaCa)
            .AsQueryable();

        if (request.TrangThaiDuyet.HasValue)
        {
            query = query.Where(x => x.TrangThaiDuyet == request.TrangThaiDuyet.Value);
        }

        if (request.NguonTaoCa.HasValue)
        {
            query = query.Where(x => x.NguonTaoCa == request.NguonTaoCa.Value);
        }

        if (request.IdChuyenKhoa.HasValue)
        {
            query = query.Where(x => x.IdChuyenKhoa == request.IdChuyenKhoa.Value);
        }

        if (request.TuNgay.HasValue)
        {
            query = query.Where(x => x.NgayLamViec >= request.TuNgay.Value);
        }

        if (request.DenNgay.HasValue)
        {
            query = query.Where(x => x.NgayLamViec <= request.DenNgay.Value);
        }

        return await query
            .OrderBy(x => x.NgayTao)
            .Skip((request.SoTrang - 1) * request.KichThuocTrang)
            .Take(request.KichThuocTrang)
            .Select(x => CaLamViecAdminResponse.TuEntity(x))
            .ToListAsync(cancellationToken);
    }
}
