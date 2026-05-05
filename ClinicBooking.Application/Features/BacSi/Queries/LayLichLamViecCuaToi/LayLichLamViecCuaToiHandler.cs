using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.Scheduling.Dtos;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.BacSi.Queries.LayLichLamViecCuaToi;

public sealed class LayLichLamViecCuaToiHandler : IRequestHandler<LayLichLamViecCuaToiQuery, IReadOnlyList<CaLamViecPublicResponse>>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public LayLichLamViecCuaToiHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<CaLamViecPublicResponse>> Handle(LayLichLamViecCuaToiQuery request, CancellationToken cancellationToken)
    {
        if (_currentUser.VaiTro != VaiTro.BacSi)
        {
            throw new ForbiddenException("Chi bac si moi duoc xem lich lam viec cua minh.");
        }

        var idTaiKhoan = _currentUser.IdTaiKhoan ?? throw new ForbiddenException("Khong xac dinh duoc tai khoan hien tai.");
        var bacSi = await _db.BacSi.AsNoTracking().FirstOrDefaultAsync(x => x.IdTaiKhoan == idTaiKhoan, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay ho so bac si.");

        return await _db.CaLamViec
            .AsNoTracking()
            .Include(x => x.BacSi)
            .Include(x => x.Phong)
            .Include(x => x.ChuyenKhoa)
            .Where(x => x.IdBacSi == bacSi.IdBacSi && x.NgayLamViec >= request.TuNgay && x.NgayLamViec <= request.DenNgay)
            .OrderBy(x => x.NgayLamViec)
            .ThenBy(x => x.GioBatDau)
            .Select(x => new CaLamViecPublicResponse(
                x.IdCaLamViec,
                x.IdBacSi,
                x.IdPhong,
                x.IdChuyenKhoa,
                x.IdDinhNghiaCa,
                x.NgayLamViec,
                x.GioBatDau,
                x.GioKetThuc,
                x.ThoiGianSlot,
                x.TrangThaiDuyet.ToString(),
                x.NguonTaoCa.ToString(),
                x.BacSi.HoTen,
                x.Phong.MaPhong,
                x.ChuyenKhoa.TenChuyenKhoa,
                x.SoSlotDaDat < x.SoSlotToiDa))
            .ToListAsync(cancellationToken);
    }
}
