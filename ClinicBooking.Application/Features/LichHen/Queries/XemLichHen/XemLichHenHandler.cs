using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.LichHen.Dtos;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.LichHen.Queries.XemLichHen;

public class XemLichHenHandler : IRequestHandler<XemLichHenQuery, LichHenResponse>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public XemLichHenHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<LichHenResponse> Handle(XemLichHenQuery request, CancellationToken cancellationToken)
    {
        var lichHen = await _db.LichHen
            .AsNoTracking()
            .Include(x => x.BenhNhan)
            .Include(x => x.CaLamViec)
            .Include(x => x.DichVu)
            .FirstOrDefaultAsync(x => x.IdLichHen == request.IdLichHen, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay lich hen.");

        // Auth: chu so huu hoac le tan/bac si/admin.
        if (_currentUser.VaiTro == VaiTro.BenhNhan)
        {
            var idTaiKhoan = _currentUser.IdTaiKhoan
                ?? throw new ForbiddenException("Khong xac dinh duoc nguoi dung hien tai.");

            var laChuSoHuu = await _db.BenhNhan
                .AsNoTracking()
                .AnyAsync(bn => bn.IdBenhNhan == lichHen.IdBenhNhan && bn.IdTaiKhoan == idTaiKhoan, cancellationToken);
            if (!laChuSoHuu)
            {
                throw new ForbiddenException("Ban khong co quyen xem lich hen nay.");
            }
        }

        return new LichHenResponse(
            lichHen.IdLichHen,
            lichHen.MaLichHen,
            lichHen.IdBenhNhan,
            lichHen.BenhNhan.HoTen,
            lichHen.IdCaLamViec,
            lichHen.CaLamViec.NgayLamViec,
            lichHen.CaLamViec.GioBatDau,
            lichHen.CaLamViec.GioKetThuc,
            lichHen.IdDichVu,
            lichHen.DichVu.TenDichVu,
            lichHen.SoSlot,
            lichHen.HinhThucDat,
            lichHen.IdBacSiMongMuon,
            lichHen.BacSiMongMuonNote,
            lichHen.TrieuChung,
            lichHen.TrangThai,
            lichHen.NgayTao);
    }
}
