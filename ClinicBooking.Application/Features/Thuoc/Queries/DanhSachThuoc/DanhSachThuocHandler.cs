using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Features.Thuoc.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.Thuoc.Queries.DanhSachThuoc;

public sealed class DanhSachThuocHandler : IRequestHandler<DanhSachThuocQuery, IReadOnlyList<ThuocResponse>>
{
    private readonly IAppDbContext _db;

    public DanhSachThuocHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<ThuocResponse>> Handle(DanhSachThuocQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Thuoc.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.TuKhoa))
        {
            query = query.Where(x =>
                x.TenThuoc.Contains(request.TuKhoa)
                || (x.HoatChat != null && x.HoatChat.Contains(request.TuKhoa)));
        }

        return await query
            .OrderBy(x => x.TenThuoc)
            .Skip((request.SoTrang - 1) * request.KichThuocTrang)
            .Take(request.KichThuocTrang)
            .Select(x => new ThuocResponse(
                x.IdThuoc,
                x.TenThuoc,
                x.HoatChat,
                x.DonVi,
                x.GhiChu))
            .ToListAsync(cancellationToken);
    }
}
