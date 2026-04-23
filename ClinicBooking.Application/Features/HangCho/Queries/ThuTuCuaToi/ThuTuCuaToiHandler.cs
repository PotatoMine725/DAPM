using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.HangCho.Dtos;
using ClinicBooking.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.HangCho.Queries.ThuTuCuaToi;

public sealed class ThuTuCuaToiHandler : IRequestHandler<ThuTuCuaToiQuery, ThuTuHangChoResponse>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public ThuTuCuaToiHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ThuTuHangChoResponse> Handle(ThuTuCuaToiQuery request, CancellationToken cancellationToken)
    {
        var idTaiKhoan = _currentUser.IdTaiKhoan
            ?? throw new ForbiddenException("Khong xac dinh duoc nguoi dung hien tai.");

        var benhNhan = await _db.BenhNhan
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdTaiKhoan == idTaiKhoan, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay ho so benh nhan tuong ung voi tai khoan.");

        var hangCho = await _db.HangCho
            .AsNoTracking()
            .Where(x => x.IdCaLamViec == request.IdCaLamViec && x.LichHen.IdBenhNhan == benhNhan.IdBenhNhan)
            .OrderByDescending(x => x.NgayCheckIn)
            .FirstOrDefaultAsync(cancellationToken);

        if (hangCho is null)
        {
            return new ThuTuHangChoResponse(
                request.IdCaLamViec,
                benhNhan.IdBenhNhan,
                0,
                0,
                false,
                null);
        }

        return new ThuTuHangChoResponse(
            request.IdCaLamViec,
            benhNhan.IdBenhNhan,
            hangCho.IdHangCho,
            hangCho.SoThuTu,
            true,
            hangCho.NgayCheckIn);
    }
}
