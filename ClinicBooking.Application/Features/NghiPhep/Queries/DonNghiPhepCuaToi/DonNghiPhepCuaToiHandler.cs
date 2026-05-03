using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Abstractions.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.NghiPhep.Queries.DonNghiPhepCuaToi;

public sealed class DonNghiPhepCuaToiHandler : IRequestHandler<DonNghiPhepCuaToiQuery, IReadOnlyList<object>>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public DonNghiPhepCuaToiHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<object>> Handle(DonNghiPhepCuaToiQuery request, CancellationToken cancellationToken)
    {
        var idTaiKhoan = _currentUser.IdTaiKhoan ?? 0;
        var bacSi = await _db.BacSi.AsNoTracking().FirstOrDefaultAsync(x => x.IdTaiKhoan == idTaiKhoan, cancellationToken);
        if (bacSi is null) return [];
        return await _db.DonNghiPhep.AsNoTracking().Where(x => x.IdBacSi == bacSi.IdBacSi).Select(x => new { x.IdDonNghiPhep }).Cast<object>().ToListAsync(cancellationToken);
    }
}
