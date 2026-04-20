using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.DanhMuc.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.DanhMuc.Queries.LayChuyenKhoaById;

public sealed class LayChuyenKhoaByIdHandler : IRequestHandler<LayChuyenKhoaByIdQuery, ChuyenKhoaResponse>
{
    private readonly IAppDbContext _db;

    public LayChuyenKhoaByIdHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<ChuyenKhoaResponse> Handle(LayChuyenKhoaByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _db.ChuyenKhoa
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdChuyenKhoa == request.IdChuyenKhoa, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay chuyen khoa.");

        return ChuyenKhoaResponse.TuEntity(entity);
    }
}
