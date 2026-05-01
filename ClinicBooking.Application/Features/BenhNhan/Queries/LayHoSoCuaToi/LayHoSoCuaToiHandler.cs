using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.BenhNhan.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.BenhNhan.Queries.LayHoSoCuaToi;

public sealed class LayHoSoCuaToiHandler : IRequestHandler<LayHoSoCuaToiQuery, BenhNhanResponse>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public LayHoSoCuaToiHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<BenhNhanResponse> Handle(LayHoSoCuaToiQuery request, CancellationToken cancellationToken)
    {
        var idTaiKhoan = _currentUser.IdTaiKhoan
            ?? throw new ForbiddenException("Khong xac dinh duoc nguoi dung hien tai.");

        var result = await _db.BenhNhan
            .AsNoTracking()
            .Where(x => x.IdTaiKhoan == idTaiKhoan)
            .Select(BenhNhanResponse.Projection)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("Khong tim thay ho so benh nhan tuong ung voi tai khoan.");

        return result;
    }
}
