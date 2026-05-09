using ClinicBooking.Application.Abstractions.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.DichVu.Queries.DanhSachDichVu;

public sealed class DanhSachDichVuHandler : IRequestHandler<DanhSachDichVuQuery, List<DichVuResponse>>
{
    private readonly IAppDbContext _db;

    public DanhSachDichVuHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<List<DichVuResponse>> Handle(DanhSachDichVuQuery request, CancellationToken cancellationToken)
    {
        return await _db.DichVu
            .AsNoTracking()
            .Where(x => x.HienThi)
            .OrderBy(x => x.TenDichVu)
            .Select(x => new DichVuResponse(x.IdDichVu, x.TenDichVu, x.IdChuyenKhoa))
            .ToListAsync(cancellationToken);
    }
}
