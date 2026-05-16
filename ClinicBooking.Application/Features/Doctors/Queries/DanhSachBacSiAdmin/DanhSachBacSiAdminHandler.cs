using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Features.Doctors.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.Doctors.Queries.DanhSachBacSiAdmin;

public sealed class DanhSachBacSiAdminHandler
    : IRequestHandler<DanhSachBacSiAdminQuery, IReadOnlyList<BacSiAdminResponse>>
{
    private readonly IAppDbContext _db;

    public DanhSachBacSiAdminHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<BacSiAdminResponse>> Handle(
        DanhSachBacSiAdminQuery request,
        CancellationToken cancellationToken)
    {
        var query = _db.BacSi
            .AsNoTracking()
            .Include(x => x.TaiKhoan)
            .Include(x => x.ChuyenKhoa)
            .AsQueryable();

        if (request.IdChuyenKhoa.HasValue)
        {
            query = query.Where(x => x.IdChuyenKhoa == request.IdChuyenKhoa.Value);
        }

        if (request.LoaiHopDong.HasValue)
        {
            query = query.Where(x => x.LoaiHopDong == request.LoaiHopDong.Value);
        }

        if (request.TrangThai.HasValue)
        {
            query = query.Where(x => x.TrangThai == request.TrangThai.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.TuKhoa))
        {
            var keyword = request.TuKhoa.Trim();
            query = query.Where(x =>
                x.HoTen.Contains(keyword) ||
                x.TaiKhoan.TenDangNhap.Contains(keyword) ||
                x.TaiKhoan.Email.Contains(keyword) ||
                x.TaiKhoan.SoDienThoai.Contains(keyword));
        }

        return await query
            .OrderByDescending(x => x.NgayTao)
            .Skip((request.SoTrang - 1) * request.KichThuocTrang)
            .Take(request.KichThuocTrang)
            .Select(x => BacSiAdminResponse.TuEntity(x))
            .ToListAsync(cancellationToken);
    }
}
