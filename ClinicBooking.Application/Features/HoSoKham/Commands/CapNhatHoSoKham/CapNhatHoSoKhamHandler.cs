using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.HoSoKham.Commands.CapNhatHoSoKham;

public sealed class CapNhatHoSoKhamHandler : IRequestHandler<CapNhatHoSoKhamCommand, Unit>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CapNhatHoSoKhamHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(CapNhatHoSoKhamCommand request, CancellationToken cancellationToken)
    {
        var idTaiKhoan = _currentUser.IdTaiKhoan
            ?? throw new ForbiddenException("Khong xac dinh duoc nguoi dung hien tai.");

        var bacSi = await _db.BacSi
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdTaiKhoan == idTaiKhoan, cancellationToken)
            ?? throw new ForbiddenException("Tai khoan hien tai khong thuoc bac si.");

        var entity = await _db.HoSoKham
            .FirstOrDefaultAsync(x => x.IdHoSoKham == request.IdHoSoKham, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay ho so kham.");

        if (entity.IdBacSi != bacSi.IdBacSi)
        {
            throw new ForbiddenException("Ban khong co quyen cap nhat ho so kham nay.");
        }

        entity.ChanDoan = string.IsNullOrWhiteSpace(request.ChanDoan) ? null : request.ChanDoan.Trim();
        entity.KetQuaKham = string.IsNullOrWhiteSpace(request.KetQuaKham) ? null : request.KetQuaKham.Trim();
        entity.GhiChu = string.IsNullOrWhiteSpace(request.GhiChu) ? null : request.GhiChu.Trim();

        await _db.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
