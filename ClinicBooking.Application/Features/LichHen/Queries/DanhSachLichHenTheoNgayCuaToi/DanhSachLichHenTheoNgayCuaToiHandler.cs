using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.LichHen.Dtos;
using ClinicBooking.Domain.Entities;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.LichHen.Queries.DanhSachLichHenTheoNgayCuaToi;

public sealed class DanhSachLichHenTheoNgayCuaToiHandler
    : IRequestHandler<DanhSachLichHenTheoNgayCuaToiQuery, IReadOnlyList<LichHenTomTatResponse>>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public DanhSachLichHenTheoNgayCuaToiHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<LichHenTomTatResponse>> Handle(
        DanhSachLichHenTheoNgayCuaToiQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.VaiTro != VaiTro.BacSi && _currentUser.VaiTro != VaiTro.Admin)
        {
            throw new ForbiddenException("Ban khong co quyen xem lich hen nay.");
        }

        IQueryable<ClinicBooking.Domain.Entities.LichHen> query = _db.LichHen
            .AsNoTracking()
            .Where(x => x.CaLamViec.NgayLamViec == request.Ngay);

        if (_currentUser.VaiTro == VaiTro.BacSi)
        {
            var idTaiKhoan = _currentUser.IdTaiKhoan
                ?? throw new ForbiddenException("Khong xac dinh duoc nguoi dung hien tai.");

            var bacSi = await _db.BacSi
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.IdTaiKhoan == idTaiKhoan, cancellationToken)
                ?? throw new NotFoundException("Khong tim thay ho so bac si tuong ung voi tai khoan.");

            query = query.Where(x => x.CaLamViec.IdBacSi == bacSi.IdBacSi);
        }

        return await query
            .OrderBy(x => x.CaLamViec.GioBatDau)
            .ThenBy(x => x.SoSlot)
            .Select(x => new LichHenTomTatResponse(
                x.IdLichHen,
                x.MaLichHen,
                x.IdBenhNhan,
                x.BenhNhan.HoTen,
                x.IdCaLamViec,
                x.CaLamViec.NgayLamViec,
                x.CaLamViec.GioBatDau,
                x.SoSlot,
                x.DichVu.TenDichVu,
                x.TrangThai,
                x.NgayTao))
            .ToListAsync(cancellationToken);
    }
}
