using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Features.BenhNhan.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.BenhNhan.Queries.DanhSachBenhNhan;

public sealed class DanhSachBenhNhanHandler : IRequestHandler<DanhSachBenhNhanQuery, IReadOnlyList<BenhNhanResponse>>
{
    private readonly IAppDbContext _db;

    public DanhSachBenhNhanHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<BenhNhanResponse>> Handle(DanhSachBenhNhanQuery request, CancellationToken cancellationToken)
    {
        var query = _db.BenhNhan
            .AsNoTracking()
            .AsQueryable();

        if (request.BiHanChe.HasValue)
        {
            query = query.Where(x => x.BiHanChe == request.BiHanChe.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.TuKhoa))
        {
            query = query.Where(x =>
                x.HoTen.Contains(request.TuKhoa)
                || (x.Cccd != null && x.Cccd.Contains(request.TuKhoa))
                || x.TaiKhoan.SoDienThoai.Contains(request.TuKhoa));
        }

        return await query
            .OrderBy(x => x.HoTen)
            .ThenBy(x => x.IdBenhNhan)
            .Skip((request.SoTrang - 1) * request.KichThuocTrang)
            .Take(request.KichThuocTrang)
            .Select(BenhNhanResponse.Projection)
            .ToListAsync(cancellationToken);
    }
}
