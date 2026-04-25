using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Features.DanhMuc.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachDichVuCongKhai;

public sealed class DanhSachDichVuCongKhaiHandler : IRequestHandler<DanhSachDichVuCongKhaiQuery, IReadOnlyList<DichVuCongKhaiResponse>>
{
    private readonly IAppDbContext _db;

    public DanhSachDichVuCongKhaiHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<DichVuCongKhaiResponse>> Handle(DanhSachDichVuCongKhaiQuery request, CancellationToken cancellationToken)
    {
        var query = _db.DichVu
            .AsNoTracking()
            .Include(x => x.ChuyenKhoa)
            .Where(x => x.HienThi && x.ChuyenKhoa.HienThi)
            .AsQueryable();

        if (request.IdChuyenKhoa.HasValue)
        {
            query = query.Where(x => x.IdChuyenKhoa == request.IdChuyenKhoa.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.TuKhoa))
        {
            query = query.Where(x => x.TenDichVu.Contains(request.TuKhoa));
        }

        return await query
            .OrderBy(x => x.TenDichVu)
            .Skip((request.SoTrang - 1) * request.KichThuocTrang)
            .Take(request.KichThuocTrang)
            .Select(x => new DichVuCongKhaiResponse(
                x.IdDichVu,
                x.IdChuyenKhoa,
                x.TenDichVu,
                x.MoTa,
                x.ThoiGianUocTinh,
                x.ChuyenKhoa.TenChuyenKhoa))
            .ToListAsync(cancellationToken);
    }
}
