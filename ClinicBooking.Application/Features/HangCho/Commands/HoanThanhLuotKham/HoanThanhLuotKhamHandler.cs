using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.HangCho.Commands.HoanThanhLuotKham;

public class HoanThanhLuotKhamHandler : IRequestHandler<HoanThanhLuotKhamCommand, Unit>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public HoanThanhLuotKhamHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(HoanThanhLuotKhamCommand request, CancellationToken cancellationToken)
    {
        var hangCho = await _db.HangCho
            .Include(x => x.LichHen)
            .Include(x => x.CaLamViec)
            .FirstOrDefaultAsync(x => x.IdHangCho == request.IdHangCho, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay ban ghi hang cho.");

        await KiemTraQuyenHoanThanhAsync(hangCho.CaLamViec.IdBacSi, cancellationToken);

        if (hangCho.TrangThai == TrangThaiHangCho.HoanThanh)
        {
            throw new ConflictException("Luot kham da duoc danh dau hoan thanh truoc do.");
        }

        if (hangCho.TrangThai != TrangThaiHangCho.DangKham)
        {
            throw new ConflictException("Chi co the hoan thanh luot dang kham.");
        }

        hangCho.TrangThai = TrangThaiHangCho.HoanThanh;
        hangCho.LichHen.TrangThai = TrangThaiLichHen.HoanThanh;

        await _db.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }

    private async Task KiemTraQuyenHoanThanhAsync(int idBacSiCa, CancellationToken cancellationToken)
    {
        // Chi bac si phu trach ca moi duoc hoan thanh luot kham.
        if (_currentUser.VaiTro != VaiTro.BacSi)
        {
            throw new ForbiddenException("Chi bac si phu trach ca moi duoc hoan thanh luot kham.");
        }

        var idTaiKhoan = _currentUser.IdTaiKhoan
            ?? throw new ForbiddenException("Khong xac dinh duoc nguoi dung hien tai.");

        var laBacSiCuaCa = await _db.BacSi
            .AsNoTracking()
            .AnyAsync(bs => bs.IdBacSi == idBacSiCa && bs.IdTaiKhoan == idTaiKhoan, cancellationToken);

        if (!laBacSiCuaCa)
        {
            throw new ForbiddenException("Ban khong phu trach ca lam viec nay.");
        }
    }
}
