using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.Thuoc.Commands.XoaThuoc;

public sealed class XoaThuocHandler : IRequestHandler<XoaThuocCommand, Unit>
{
    private readonly IAppDbContext _db;

    public XoaThuocHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Unit> Handle(XoaThuocCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Thuoc
            .FirstOrDefaultAsync(x => x.IdThuoc == request.IdThuoc, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay thuoc.");

        var dangDuocSuDung = await _db.ToaThuoc
            .AnyAsync(x => x.IdThuoc == request.IdThuoc, cancellationToken);
        if (dangDuocSuDung)
        {
            throw new ConflictException("Khong the xoa thuoc dang duoc su dung trong toa thuoc.");
        }

        _db.Thuoc.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
