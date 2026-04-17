using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.LichHen.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.LichHen.Queries.DanhSachLichHenCuaToi;

public class DanhSachLichHenCuaToiHandler : IRequestHandler<DanhSachLichHenCuaToiQuery, DanhSachLichHenResponse>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public DanhSachLichHenCuaToiHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<DanhSachLichHenResponse> Handle(
        DanhSachLichHenCuaToiQuery request,
        CancellationToken cancellationToken)
    {
        var idTaiKhoan = _currentUser.IdTaiKhoan
            ?? throw new ForbiddenException("Khong xac dinh duoc nguoi dung hien tai.");

        var benhNhan = await _db.BenhNhan
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdTaiKhoan == idTaiKhoan, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay ho so benh nhan tuong ung voi tai khoan.");

        var query = _db.LichHen
            .AsNoTracking()
            .Where(x => x.IdBenhNhan == benhNhan.IdBenhNhan);

        if (request.TrangThai.HasValue)
        {
            query = query.Where(x => x.TrangThai == request.TrangThai.Value);
        }

        var tongSo = await query.CountAsync(cancellationToken);

        var ketQua = await query
            .OrderByDescending(x => x.NgayTao)
            .Skip((request.SoTrang - 1) * request.KichThuocTrang)
            .Take(request.KichThuocTrang)
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

        return new DanhSachLichHenResponse(ketQua, tongSo, request.SoTrang, request.KichThuocTrang);
    }
}
