using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.BenhNhan.Commands.CapNhatHoSoCuaToi;

public sealed class CapNhatHoSoCuaToiHandler : IRequestHandler<CapNhatHoSoCuaToiCommand, Unit>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CapNhatHoSoCuaToiHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(CapNhatHoSoCuaToiCommand request, CancellationToken cancellationToken)
    {
        var idTaiKhoan = _currentUser.IdTaiKhoan
            ?? throw new ForbiddenException("Khong xac dinh duoc nguoi dung hien tai.");

        var entity = await _db.BenhNhan
            .FirstOrDefaultAsync(x => x.IdTaiKhoan == idTaiKhoan, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay ho so benh nhan tuong ung voi tai khoan.");

        var cccdMoi = string.IsNullOrWhiteSpace(request.Cccd) ? null : request.Cccd.Trim();
        if (!string.IsNullOrWhiteSpace(cccdMoi))
        {
            var cccdTrung = await _db.BenhNhan
                .AnyAsync(x => x.IdBenhNhan != entity.IdBenhNhan && x.Cccd == cccdMoi, cancellationToken);
            if (cccdTrung)
            {
                throw new ConflictException("CCCD da duoc su dung.");
            }
        }

        entity.HoTen = request.HoTen.Trim();
        entity.NgaySinh = request.NgaySinh;
        entity.GioiTinh = request.GioiTinh;
        entity.Cccd = cccdMoi;
        entity.DiaChi = string.IsNullOrWhiteSpace(request.DiaChi) ? null : request.DiaChi.Trim();

        await _db.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
