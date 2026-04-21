using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.DanhMuc.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.DanhMuc.Queries.LayDinhNghiaCaById;

public sealed class LayDinhNghiaCaByIdHandler : IRequestHandler<LayDinhNghiaCaByIdQuery, DinhNghiaCaResponse>
{
    private readonly IAppDbContext _db;

    public LayDinhNghiaCaByIdHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<DinhNghiaCaResponse> Handle(LayDinhNghiaCaByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _db.DinhNghiaCa
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdDinhNghiaCa == request.IdDinhNghiaCa, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay dinh nghia ca.");

        return DinhNghiaCaResponse.TuEntity(entity);
    }
}
