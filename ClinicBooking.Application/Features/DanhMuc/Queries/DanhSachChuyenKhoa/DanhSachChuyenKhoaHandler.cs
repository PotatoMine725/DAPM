using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Features.DanhMuc.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachChuyenKhoa;

public sealed class DanhSachChuyenKhoaHandler : IRequestHandler<DanhSachChuyenKhoaQuery, IReadOnlyList<ChuyenKhoaResponse>>
{
    private readonly IAppDbContext _db;

    public DanhSachChuyenKhoaHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<ChuyenKhoaResponse>> Handle(DanhSachChuyenKhoaQuery request, CancellationToken cancellationToken)
    {
        var query = _db.ChuyenKhoa.AsNoTracking().AsQueryable();

        if (request.HienThi.HasValue)
        {
            query = query.Where(x => x.HienThi == request.HienThi.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.TuKhoa))
        {
            query = query.Where(x => x.TenChuyenKhoa.Contains(request.TuKhoa));
        }

        return await query
            .OrderBy(x => x.TenChuyenKhoa)
            .Skip((request.SoTrang - 1) * request.KichThuocTrang)
            .Take(request.KichThuocTrang)
            .Select(x => new ChuyenKhoaResponse(
                x.IdChuyenKhoa,
                x.TenChuyenKhoa,
                x.MoTa,
                x.ThoiGianSlotMacDinh,
                x.GioMoDatLich,
                x.GioDongDatLich,
                x.HienThi))
            .ToListAsync(cancellationToken);
    }
}
