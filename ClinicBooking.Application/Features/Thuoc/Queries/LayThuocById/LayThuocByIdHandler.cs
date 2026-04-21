using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.Thuoc.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.Thuoc.Queries.LayThuocById;

public sealed class LayThuocByIdHandler : IRequestHandler<LayThuocByIdQuery, ThuocResponse>
{
    private readonly IAppDbContext _db;

    public LayThuocByIdHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<ThuocResponse> Handle(LayThuocByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _db.Thuoc
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdThuoc == request.IdThuoc, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay thuoc.");

        return ThuocResponse.TuEntity(entity);
    }
}
