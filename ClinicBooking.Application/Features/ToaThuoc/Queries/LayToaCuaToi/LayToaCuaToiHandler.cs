using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.ToaThuoc.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.ToaThuoc.Queries.LayToaCuaToi;

public sealed class LayToaCuaToiHandler : IRequestHandler<LayToaCuaToiQuery, IReadOnlyList<ToaThuocResponse>>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public LayToaCuaToiHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<ToaThuocResponse>> Handle(
        LayToaCuaToiQuery request,
        CancellationToken cancellationToken)
    {
        var idTaiKhoan = _currentUser.IdTaiKhoan
            ?? throw new ForbiddenException("Khong xac dinh duoc nguoi dung hien tai.");

        var benhNhan = await _db.BenhNhan
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdTaiKhoan == idTaiKhoan, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay ho so benh nhan tuong ung voi tai khoan.");

        return await _db.ToaThuoc
            .AsNoTracking()
            .Include(x => x.Thuoc)
            .Include(x => x.HoSoKham).ThenInclude(x => x.LichHen)
            .Where(x => x.HoSoKham.LichHen.IdBenhNhan == benhNhan.IdBenhNhan)
            .OrderByDescending(x => x.HoSoKham.NgayKham)
            .ThenBy(x => x.IdToaThuoc)
            .Skip((request.SoTrang - 1) * request.KichThuocTrang)
            .Take(request.KichThuocTrang)
            .Select(x => new ToaThuocResponse(
                x.IdToaThuoc,
                x.IdHoSoKham,
                x.IdThuoc,
                x.Thuoc.TenThuoc,
                x.LieuLuong,
                x.CachDung,
                x.SoNgayDung,
                x.GhiChu,
                x.HoSoKham.NgayKham,
                x.HoSoKham.LichHen.MaLichHen))
            .ToListAsync(cancellationToken);
    }
}
