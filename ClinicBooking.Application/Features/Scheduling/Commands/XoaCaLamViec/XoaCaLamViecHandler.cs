using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.Scheduling.Commands.XoaCaLamViec;

public sealed class XoaCaLamViecHandler : IRequestHandler<XoaCaLamViecCommand>
{
    private readonly IAppDbContext _db;

    public XoaCaLamViecHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task Handle(XoaCaLamViecCommand request, CancellationToken cancellationToken)
    {
        var ca = await _db.CaLamViec.FirstOrDefaultAsync(x => x.IdCaLamViec == request.IdCaLamViec, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay ca lam viec.");

        if (ca.SoSlotDaDat > 0)
        {
            throw new ConflictException("Khong the xoa ca da co lich hen.");
        }

        if (ca.TrangThaiDuyet == TrangThaiDuyetCa.DaDuyet)
        {
            throw new ConflictException("Khong the xoa ca da duyet.");
        }

        _db.CaLamViec.Remove(ca);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
