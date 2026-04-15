using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.Auth.Commands.DangXuat;

public class DangXuatHandler : IRequestHandler<DangXuatCommand>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _dateTimeProvider;

    public DangXuatHandler(
        IAppDbContext db,
        ICurrentUserService currentUser,
        IDateTimeProvider dateTimeProvider)
    {
        _db = db;
        _currentUser = currentUser;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task Handle(
        DangXuatCommand request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.DaXacThuc || _currentUser.IdTaiKhoan is null)
        {
            throw new UnauthorizedAccessException("Chua xac thuc.");
        }

        var token = await _db.RefreshToken
            .FirstOrDefaultAsync(x => x.Token == request.RefreshToken, cancellationToken);

        if (token is null)
        {
            // Im lang: dang xuat la hanh dong idempotent.
            return;
        }

        if (token.IdTaiKhoan != _currentUser.IdTaiKhoan.Value)
        {
            throw new ForbiddenException("Khong co quyen thu hoi token nay.");
        }

        if (token.DaThuHoi)
        {
            return;
        }

        token.DaThuHoi = true;
        token.NgayThuHoi = _dateTimeProvider.UtcNow;
        token.LyDoThuHoi = "Nguoi dung dang xuat.";

        await _db.SaveChangesAsync(cancellationToken);
    }
}
