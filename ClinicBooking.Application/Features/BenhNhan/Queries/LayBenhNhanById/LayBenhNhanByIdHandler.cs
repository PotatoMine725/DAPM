using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.BenhNhan.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.BenhNhan.Queries.LayBenhNhanById;

public sealed class LayBenhNhanByIdHandler : IRequestHandler<LayBenhNhanByIdQuery, BenhNhanResponse>
{
    private readonly IAppDbContext _db;

    public LayBenhNhanByIdHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<BenhNhanResponse> Handle(LayBenhNhanByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await _db.BenhNhan
            .AsNoTracking()
            .Where(x => x.IdBenhNhan == request.IdBenhNhan)
            .Select(BenhNhanResponse.Projection)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("Khong tim thay benh nhan.");

        return result;
    }
}
