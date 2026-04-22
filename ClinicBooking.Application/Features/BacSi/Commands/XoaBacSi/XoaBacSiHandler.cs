using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.BacSi.Commands.XoaBacSi;

public sealed class XoaBacSiHandler : IRequestHandler<XoaBacSiCommand>
{
    private readonly IAppDbContext _db;

    public XoaBacSiHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task Handle(XoaBacSiCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.BacSi.FirstOrDefaultAsync(x => x.IdBacSi == request.IdBacSi, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay bac si.");

        _db.BacSi.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
