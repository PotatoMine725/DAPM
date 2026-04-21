using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.HoSoKham.Dtos;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.HoSoKham.Queries.LayHoSoKhamById;

public sealed class LayHoSoKhamByIdHandler : IRequestHandler<LayHoSoKhamByIdQuery, HoSoKhamResponse>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public LayHoSoKhamByIdHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<HoSoKhamResponse> Handle(LayHoSoKhamByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _db.HoSoKham
            .AsNoTracking()
            .Include(x => x.BacSi)
            .Include(x => x.LichHen).ThenInclude(x => x.BenhNhan).ThenInclude(x => x.TaiKhoan)
            .FirstOrDefaultAsync(x => x.IdHoSoKham == request.IdHoSoKham, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay ho so kham.");

        await KiemTraQuyenXemAsync(entity, cancellationToken);
        return HoSoKhamResponse.TuEntity(entity);
    }

    private async Task KiemTraQuyenXemAsync(ClinicBooking.Domain.Entities.HoSoKham entity, CancellationToken cancellationToken)
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
            if (entity.LichHen.BenhNhan.IdTaiKhoan != idTaiKhoan)
            {
                throw new ForbiddenException("Ban khong co quyen xem ho so kham nay.");
            }

            return;
        }

        if (_currentUser.VaiTro == VaiTro.BacSi)
        {
            var bacSi = await _db.BacSi
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.IdTaiKhoan == idTaiKhoan, cancellationToken)
                ?? throw new ForbiddenException("Tai khoan hien tai khong thuoc bac si.");

            if (entity.IdBacSi != bacSi.IdBacSi)
            {
                throw new ForbiddenException("Ban khong co quyen xem ho so kham nay.");
            }

            return;
        }

        throw new ForbiddenException("Vai tro hien tai khong duoc phep truy cap.");
    }
}
