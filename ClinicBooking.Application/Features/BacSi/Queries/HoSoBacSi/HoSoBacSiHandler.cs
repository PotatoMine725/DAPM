using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.BacSi.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.BacSi.Queries.HoSoBacSi;

public sealed class HoSoBacSiHandler : IRequestHandler<HoSoBacSiQuery, BacSiResponse>
{
    private readonly IAppDbContext _db;

    public HoSoBacSiHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<BacSiResponse> Handle(HoSoBacSiQuery request, CancellationToken cancellationToken)
    {
        var entity = await _db.BacSi
            .AsNoTracking()
            .Include(x => x.ChuyenKhoa)
            .FirstOrDefaultAsync(x => x.IdBacSi == request.IdBacSi, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay bac si.");

        return BacSiResponse.TuEntity(entity);
    }
}
