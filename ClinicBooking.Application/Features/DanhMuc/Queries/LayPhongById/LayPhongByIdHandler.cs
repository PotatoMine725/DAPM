using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.DanhMuc.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.DanhMuc.Queries.LayPhongById;

public sealed class LayPhongByIdHandler : IRequestHandler<LayPhongByIdQuery, PhongResponse>
{
    private readonly IAppDbContext _db;

    public LayPhongByIdHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<PhongResponse> Handle(LayPhongByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _db.Phong
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdPhong == request.IdPhong, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay phong.");

        return PhongResponse.TuEntity(entity);
    }
}
