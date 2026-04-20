using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Features.DanhMuc.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachDichVu;

public sealed class DanhSachDichVuHandler : IRequestHandler<DanhSachDichVuQuery, IReadOnlyList<DichVuResponse>>
{
    private readonly IAppDbContext _db;

    public DanhSachDichVuHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<DichVuResponse>> Handle(DanhSachDichVuQuery request, CancellationToken cancellationToken)
    {
        var query = _db.DichVu
            .AsNoTracking()
            .Include(x => x.ChuyenKhoa)
            .AsQueryable();

        if (request.IdChuyenKhoa.HasValue)
        {
            query = query.Where(x => x.IdChuyenKhoa == request.IdChuyenKhoa.Value);
        }

        if (request.HienThi.HasValue)
        {
            query = query.Where(x => x.HienThi == request.HienThi.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.TuKhoa))
        {
            query = query.Where(x => x.TenDichVu.Contains(request.TuKhoa));
        }

        return await query
            .OrderBy(x => x.TenDichVu)
            .Skip((request.SoTrang - 1) * request.KichThuocTrang)
            .Take(request.KichThuocTrang)
            .Select(x => new DichVuResponse(
                x.IdDichVu,
                x.IdChuyenKhoa,
                x.TenDichVu,
                x.MoTa,
                x.ThoiGianUocTinh,
                x.HienThi,
                x.NgayTao,
                x.ChuyenKhoa.TenChuyenKhoa))
            .ToListAsync(cancellationToken);
    }
}
