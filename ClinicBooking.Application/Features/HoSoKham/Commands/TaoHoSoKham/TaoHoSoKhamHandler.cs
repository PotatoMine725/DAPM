using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.HoSoKham.Commands.TaoHoSoKham;

public sealed class TaoHoSoKhamHandler : IRequestHandler<TaoHoSoKhamCommand, int>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _dateTimeProvider;

    public TaoHoSoKhamHandler(
        IAppDbContext db,
        ICurrentUserService currentUser,
        IDateTimeProvider dateTimeProvider)
    {
        _db = db;
        _currentUser = currentUser;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<int> Handle(TaoHoSoKhamCommand request, CancellationToken cancellationToken)
    {
        var idTaiKhoan = _currentUser.IdTaiKhoan
            ?? throw new ForbiddenException("Khong xac dinh duoc nguoi dung hien tai.");

        var bacSi = await _db.BacSi
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdTaiKhoan == idTaiKhoan, cancellationToken)
            ?? throw new ForbiddenException("Tai khoan hien tai khong thuoc bac si.");

        var lichHen = await _db.LichHen
            .Include(x => x.CaLamViec)
            .FirstOrDefaultAsync(x => x.IdLichHen == request.IdLichHen, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay lich hen.");

        if (lichHen.CaLamViec.IdBacSi != bacSi.IdBacSi)
        {
            throw new ForbiddenException("Ban khong co quyen tao ho so cho lich hen nay.");
        }

        if (lichHen.TrangThai is not (TrangThaiLichHen.DangKham or TrangThaiLichHen.HoanThanh))
        {
            throw new ConflictException("Chi co the tao ho so cho lich hen dang kham hoac da hoan thanh.");
        }

        var daTonTai = await _db.HoSoKham
            .AnyAsync(x => x.IdLichHen == request.IdLichHen, cancellationToken);
        if (daTonTai)
        {
            throw new ConflictException("Lich hen nay da co ho so kham.");
        }

        var now = _dateTimeProvider.UtcNow;
        var entity = new ClinicBooking.Domain.Entities.HoSoKham
        {
            IdLichHen = request.IdLichHen,
            IdBacSi = bacSi.IdBacSi,
            ChanDoan = string.IsNullOrWhiteSpace(request.ChanDoan) ? null : request.ChanDoan.Trim(),
            KetQuaKham = string.IsNullOrWhiteSpace(request.KetQuaKham) ? null : request.KetQuaKham.Trim(),
            GhiChu = string.IsNullOrWhiteSpace(request.GhiChu) ? null : request.GhiChu.Trim(),
            NgayKham = now,
            NgayTao = now
        };

        _db.HoSoKham.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return entity.IdHoSoKham;
    }
}
