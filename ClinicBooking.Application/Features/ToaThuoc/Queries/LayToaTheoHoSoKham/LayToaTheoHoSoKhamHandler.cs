using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.ToaThuoc.Dtos;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.ToaThuoc.Queries.LayToaTheoHoSoKham;

public sealed class LayToaTheoHoSoKhamHandler
    : IRequestHandler<LayToaTheoHoSoKhamQuery, IReadOnlyList<ToaThuocResponse>>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public LayToaTheoHoSoKhamHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<ToaThuocResponse>> Handle(
        LayToaTheoHoSoKhamQuery request,
        CancellationToken cancellationToken)
    {
        var hoSo = await _db.HoSoKham
            .AsNoTracking()
            .Include(x => x.LichHen).ThenInclude(x => x.BenhNhan).ThenInclude(x => x.TaiKhoan)
            .FirstOrDefaultAsync(x => x.IdHoSoKham == request.IdHoSoKham, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay ho so kham.");

        await KiemTraQuyenXemAsync(hoSo, cancellationToken);

        return await _db.ToaThuoc
            .AsNoTracking()
            .Include(x => x.Thuoc)
            .Include(x => x.HoSoKham).ThenInclude(x => x.LichHen)
            .Where(x => x.IdHoSoKham == request.IdHoSoKham)
            .OrderBy(x => x.IdToaThuoc)
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

    private async Task KiemTraQuyenXemAsync(ClinicBooking.Domain.Entities.HoSoKham hoSo, CancellationToken cancellationToken)
    {
        if (_currentUser.VaiTro is null)
        {
            throw new ForbiddenException("Khong xac dinh duoc vai tro hien tai.");
        }

        if (_currentUser.VaiTro == VaiTro.Admin || _currentUser.VaiTro == VaiTro.LeTan)
        {
            return;
        }

        var idTaiKhoan = _currentUser.IdTaiKhoan
            ?? throw new ForbiddenException("Khong xac dinh duoc nguoi dung hien tai.");

        if (_currentUser.VaiTro == VaiTro.BenhNhan)
        {
            if (hoSo.LichHen.BenhNhan.IdTaiKhoan != idTaiKhoan)
            {
                throw new ForbiddenException("Ban khong co quyen xem toa thuoc nay.");
            }

            return;
        }

        if (_currentUser.VaiTro == VaiTro.BacSi)
        {
            var bacSi = await _db.BacSi
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.IdTaiKhoan == idTaiKhoan, cancellationToken)
                ?? throw new ForbiddenException("Tai khoan hien tai khong thuoc bac si.");

            if (hoSo.IdBacSi != bacSi.IdBacSi)
            {
                throw new ForbiddenException("Ban khong co quyen xem toa thuoc nay.");
            }

            return;
        }

        throw new ForbiddenException("Vai tro hien tai khong duoc phep truy cap.");
    }
}
