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

        if (request.IdChuyenKhoa.HasValue)
        {
            query = query.Where(x => x.IdChuyenKhoa == request.IdChuyenKhoa.Value);
        }

        // Mac dinh chi hien bac si dang lam viec cho portal benh nhan.
        // Truyen DangLamViec=false de lay bac si da nghi viec (admin use case).
        if (request.DangLamViec.HasValue)
        {
            query = query.Where(x => (x.TrangThai == TrangThaiBacSi.DangLam) == request.DangLamViec.Value);
        }
        else
        {
            query = query.Where(x => x.TrangThai == TrangThaiBacSi.DangLam);
        }

        // Public portal should still prefer visible departments, but do not hard-drop
        // doctors if the data set has not been fully synced yet.
        query = query.Where(x => x.ChuyenKhoa.HienThi);

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
