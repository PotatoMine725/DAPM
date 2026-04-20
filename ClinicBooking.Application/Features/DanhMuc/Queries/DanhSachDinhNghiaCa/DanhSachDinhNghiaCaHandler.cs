using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Features.DanhMuc.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachDinhNghiaCa;

public sealed class DanhSachDinhNghiaCaHandler : IRequestHandler<DanhSachDinhNghiaCaQuery, IReadOnlyList<DinhNghiaCaResponse>>
{
    private readonly IAppDbContext _db;

    public DanhSachDinhNghiaCaHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<DinhNghiaCaResponse>> Handle(DanhSachDinhNghiaCaQuery request, CancellationToken cancellationToken)
    {
        var query = _db.DinhNghiaCa.AsNoTracking().AsQueryable();

        if (request.TrangThai.HasValue)
        {
            query = query.Where(x => x.TrangThai == request.TrangThai.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.TuKhoa))
        {
            query = query.Where(x => x.TenCa.Contains(request.TuKhoa));
        }

        return await query
            .OrderBy(x => x.GioBatDauMacDinh)
            .Skip((request.SoTrang - 1) * request.KichThuocTrang)
            .Take(request.KichThuocTrang)
            .Select(x => new DinhNghiaCaResponse(
                x.IdDinhNghiaCa,
                x.TenCa,
                x.GioBatDauMacDinh,
                x.GioKetThucMacDinh,
                x.MoTa,
                x.TrangThai))
            .ToListAsync(cancellationToken);
    }
}
