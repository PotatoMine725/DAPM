using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.HoSoKham.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.HoSoKham.Queries.LichSuKhamCuaToi;

public sealed class LichSuKhamCuaToiHandler : IRequestHandler<LichSuKhamCuaToiQuery, IReadOnlyList<HoSoKhamTomTatResponse>>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public LichSuKhamCuaToiHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<HoSoKhamTomTatResponse>> Handle(
        LichSuKhamCuaToiQuery request,
        CancellationToken cancellationToken)
    {
        var idTaiKhoan = _currentUser.IdTaiKhoan
            ?? throw new ForbiddenException("Khong xac dinh duoc nguoi dung hien tai.");

        var benhNhan = await _db.BenhNhan
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdTaiKhoan == idTaiKhoan, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay ho so benh nhan tuong ung voi tai khoan.");

        return await _db.HoSoKham
            .AsNoTracking()
            .Include(x => x.BacSi)
            .Include(x => x.LichHen)
            .Where(x => x.LichHen.IdBenhNhan == benhNhan.IdBenhNhan)
            .OrderByDescending(x => x.NgayKham)
            .Skip((request.SoTrang - 1) * request.KichThuocTrang)
            .Take(request.KichThuocTrang)
            .Select(x => new HoSoKhamTomTatResponse(
                x.IdHoSoKham,
                x.IdLichHen,
                x.LichHen.MaLichHen,
                x.IdBacSi,
                x.BacSi.HoTen,
                x.NgayKham,
                x.ChanDoan,
                x.KetQuaKham,
                x.NgayTao))
            .ToListAsync(cancellationToken);
    }
}
