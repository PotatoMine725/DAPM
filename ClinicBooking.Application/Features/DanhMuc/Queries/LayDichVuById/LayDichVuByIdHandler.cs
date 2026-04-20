using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.DanhMuc.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.DanhMuc.Queries.LayDichVuById;

public sealed class LayDichVuByIdHandler : IRequestHandler<LayDichVuByIdQuery, DichVuResponse>
{
    private readonly IAppDbContext _db;

    public LayDichVuByIdHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<DichVuResponse> Handle(LayDichVuByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _db.DichVu
            .AsNoTracking()
            .Include(x => x.ChuyenKhoa)
            .FirstOrDefaultAsync(x => x.IdDichVu == request.IdDichVu, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay dich vu.");

        return DichVuResponse.TuEntity(entity);
    }
}
