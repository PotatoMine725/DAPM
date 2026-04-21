using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.Thuoc.Commands.CapNhatThuoc;

public sealed class CapNhatThuocHandler : IRequestHandler<CapNhatThuocCommand, Unit>
{
    private readonly IAppDbContext _db;

    public CapNhatThuocHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Unit> Handle(CapNhatThuocCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Thuoc
            .FirstOrDefaultAsync(x => x.IdThuoc == request.IdThuoc, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay thuoc.");

        var tenDaTonTai = await _db.Thuoc
            .AnyAsync(x => x.IdThuoc != request.IdThuoc && x.TenThuoc == request.TenThuoc, cancellationToken);
        if (tenDaTonTai)
        {
            throw new ConflictException("Ten thuoc da ton tai.");
        }

        entity.TenThuoc = request.TenThuoc;
        entity.HoatChat = request.HoatChat;
        entity.DonVi = request.DonVi;
        entity.GhiChu = request.GhiChu;

        await _db.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
