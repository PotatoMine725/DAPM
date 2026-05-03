using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.BacSi.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.BacSi.Queries.LayHoSoCuaToi;

public sealed class LayHoSoCuaToiHandler : IRequestHandler<LayHoSoCuaToiQuery, BacSiResponse>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public LayHoSoCuaToiHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<BacSiResponse> Handle(LayHoSoCuaToiQuery request, CancellationToken cancellationToken)
    {
        var idTaiKhoan = _currentUser.IdTaiKhoan ?? throw new ForbiddenException("Khong xac dinh duoc tai khoan hien tai.");

        var entity = await _db.BacSi
            .AsNoTracking()
            .Include(x => x.ChuyenKhoa)
            .FirstOrDefaultAsync(x => x.IdTaiKhoan == idTaiKhoan, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay ho so bac si.");

        return BacSiResponse.TuEntity(entity);
    }
}
