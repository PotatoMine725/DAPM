using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Features.Doctors.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.Doctors.Queries.LayHoSoBacSiCuaToi;

public sealed class LayHoSoBacSiCuaToiHandler : IRequestHandler<LayHoSoBacSiCuaToiQuery, HoSoBacSiCuaToiResponse?>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public LayHoSoBacSiCuaToiHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<HoSoBacSiCuaToiResponse?> Handle(LayHoSoBacSiCuaToiQuery request, CancellationToken cancellationToken)
    {
        var idTaiKhoan = _currentUser.IdTaiKhoan;
        if (idTaiKhoan is null) return null;

        return await _db.BacSi
            .AsNoTracking()
            .Where(x => x.IdTaiKhoan == idTaiKhoan)
            .Select(x => new HoSoBacSiCuaToiResponse(
                x.IdBacSi,
                x.HoTen,
                x.ChuyenKhoa.TenChuyenKhoa,
                x.LoaiHopDong))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
