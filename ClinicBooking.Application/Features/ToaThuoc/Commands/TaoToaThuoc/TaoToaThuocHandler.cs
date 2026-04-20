using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.ToaThuoc.Commands.TaoToaThuoc;

public sealed class TaoToaThuocHandler : IRequestHandler<TaoToaThuocCommand, Unit>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public TaoToaThuocHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(TaoToaThuocCommand request, CancellationToken cancellationToken)
    {
        var bacSi = await LayBacSiHienTaiAsync(cancellationToken);
        var hoSoKham = await _db.HoSoKham
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdHoSoKham == request.IdHoSoKham, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay ho so kham.");

        if (hoSoKham.IdBacSi != bacSi.IdBacSi)
        {
            throw new ForbiddenException("Ban khong co quyen ke toa cho ho so kham nay.");
        }

        var daCoToa = await _db.ToaThuoc
            .AnyAsync(x => x.IdHoSoKham == request.IdHoSoKham, cancellationToken);
        if (daCoToa)
        {
            throw new ConflictException("Ho so kham nay da co toa thuoc.");
        }

        var idThuocKhongTrung = request.DanhSachThuoc.Select(x => x.IdThuoc).Distinct().ToList();
        if (idThuocKhongTrung.Count != request.DanhSachThuoc.Count)
        {
            throw new ConflictException("Don thuoc khong duoc co thuoc trung lap.");
        }

        var soThuocTonTai = await _db.Thuoc
            .CountAsync(x => idThuocKhongTrung.Contains(x.IdThuoc), cancellationToken);
        if (soThuocTonTai != idThuocKhongTrung.Count)
        {
            throw new NotFoundException("Co thuoc khong ton tai trong danh sach ke don.");
        }

        foreach (var item in request.DanhSachThuoc)
        {
            _db.ToaThuoc.Add(new ClinicBooking.Domain.Entities.ToaThuoc
            {
                IdHoSoKham = request.IdHoSoKham,
                IdThuoc = item.IdThuoc,
                LieuLuong = string.IsNullOrWhiteSpace(item.LieuLuong) ? null : item.LieuLuong.Trim(),
                CachDung = string.IsNullOrWhiteSpace(item.CachDung) ? null : item.CachDung.Trim(),
                SoNgayDung = item.SoNgayDung,
                GhiChu = string.IsNullOrWhiteSpace(item.GhiChu) ? null : item.GhiChu.Trim()
            });
        }

        await _db.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }

    private async Task<ClinicBooking.Domain.Entities.BacSi> LayBacSiHienTaiAsync(CancellationToken cancellationToken)
    {
        var idTaiKhoan = _currentUser.IdTaiKhoan
            ?? throw new ForbiddenException("Khong xac dinh duoc nguoi dung hien tai.");

        return await _db.BacSi
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdTaiKhoan == idTaiKhoan, cancellationToken)
            ?? throw new ForbiddenException("Tai khoan hien tai khong thuoc bac si.");
    }
}
