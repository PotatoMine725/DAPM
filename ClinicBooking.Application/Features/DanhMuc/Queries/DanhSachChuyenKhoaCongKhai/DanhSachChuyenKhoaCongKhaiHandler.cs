using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Features.DanhMuc.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachChuyenKhoaCongKhai;

public sealed class DanhSachChuyenKhoaCongKhaiHandler : IRequestHandler<DanhSachChuyenKhoaCongKhaiQuery, IReadOnlyList<ChuyenKhoaCongKhaiResponse>>
{
    private readonly IAppDbContext _db;

    public DanhSachChuyenKhoaCongKhaiHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<ChuyenKhoaCongKhaiResponse>> Handle(DanhSachChuyenKhoaCongKhaiQuery request, CancellationToken cancellationToken)
    {
        var query = _db.ChuyenKhoa
            .AsNoTracking()
            .Where(x => x.HienThi)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.TuKhoa))
        {
            query = query.Where(x => x.TenChuyenKhoa.Contains(request.TuKhoa));
        }

        return await query
            .OrderBy(x => x.TenChuyenKhoa)
            .Skip((request.SoTrang - 1) * request.KichThuocTrang)
            .Take(request.KichThuocTrang)
            .Select(x => new ChuyenKhoaCongKhaiResponse(
                x.IdChuyenKhoa,
                x.TenChuyenKhoa,
                x.MoTa,
                x.ThoiGianSlotMacDinh,
                x.GioMoDatLich,
                x.GioDongDatLich))
            .ToListAsync(cancellationToken);
    }
}
