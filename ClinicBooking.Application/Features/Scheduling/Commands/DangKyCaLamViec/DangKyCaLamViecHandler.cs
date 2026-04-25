using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Domain.Entities;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.Scheduling.Commands.DangKyCaLamViec;

public sealed class DangKyCaLamViecHandler : IRequestHandler<DangKyCaLamViecCommand, int>
{
    private readonly IAppDbContext _db;

    public DangKyCaLamViecHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<int> Handle(DangKyCaLamViecCommand request, CancellationToken cancellationToken)
    {
        var caLamViec = await _db.CaLamViec
            .Include(x => x.BacSi)
            .FirstOrDefaultAsync(x => x.IdCaLamViec == request.IdCaLamViec, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay ca lam viec.");

        if (caLamViec.TrangThaiDuyet != TrangThaiDuyetCa.ChoDuyet)
        {
            throw new ConflictException("Ca lam viec da duoc xu ly.");
        }

        caLamViec.NguonTaoCa = NguonTaoCa.BacSiDangKy;
        caLamViec.IdBacSiYeuCau ??= caLamViec.IdBacSi;
        await _db.SaveChangesAsync(cancellationToken);
        return caLamViec.IdCaLamViec;
    }
}
