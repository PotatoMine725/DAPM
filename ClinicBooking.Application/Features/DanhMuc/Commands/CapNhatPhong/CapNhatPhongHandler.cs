using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.DanhMuc.Commands.CapNhatPhong;

public sealed class CapNhatPhongHandler : IRequestHandler<CapNhatPhongCommand, Unit>
{
    private readonly IAppDbContext _db;

    public CapNhatPhongHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Unit> Handle(CapNhatPhongCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Phong
            .FirstOrDefaultAsync(x => x.IdPhong == request.IdPhong, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay phong.");

        var maPhongDaTonTai = await _db.Phong
            .AnyAsync(x => x.IdPhong != request.IdPhong && x.MaPhong == request.MaPhong, cancellationToken);
        if (maPhongDaTonTai)
        {
            throw new ConflictException("Ma phong da ton tai.");
        }

        entity.MaPhong = request.MaPhong;
        entity.TenPhong = request.TenPhong;
        entity.SucChua = request.SucChua;
        entity.TrangBi = request.TrangBi;
        entity.TrangThai = request.TrangThai;

        await _db.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
