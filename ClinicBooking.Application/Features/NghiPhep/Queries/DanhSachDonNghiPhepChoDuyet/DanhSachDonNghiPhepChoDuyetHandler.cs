using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Features.NghiPhep.Dtos;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.NghiPhep.Queries.DanhSachDonNghiPhepChoDuyet;

public sealed class DanhSachDonNghiPhepChoDuyetHandler : IRequestHandler<DanhSachDonNghiPhepChoDuyetQuery, IReadOnlyList<DonNghiPhepResponse>>
{
    private readonly IAppDbContext _db;

    public DanhSachDonNghiPhepChoDuyetHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<DonNghiPhepResponse>> Handle(DanhSachDonNghiPhepChoDuyetQuery request, CancellationToken cancellationToken)
    {
        return await _db.DonNghiPhep
            .AsNoTracking()
            .Include(x => x.BacSi)
                .ThenInclude(x => x.ChuyenKhoa)
            .Include(x => x.CaLamViec)
            .Where(x => x.TrangThaiDuyet == TrangThaiDuyetDon.ChoDuyet)
            .OrderByDescending(x => x.NgayGuiDon)
            .Select(x => DonNghiPhepResponse.TuEntity(x))
            .ToListAsync(cancellationToken);
    }
}
