using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Domain.Enums;
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

        var homNay = DateOnly.FromDateTime(DateTime.UtcNow);
        var conCaLamViecSapToi = await _db.CaLamViec.AnyAsync(
            x => x.IdBacSi == request.IdBacSi
                && x.TrangThaiDuyet == TrangThaiDuyetCa.DaDuyet
                && x.NgayLamViec > homNay,
            cancellationToken);

        if (conCaLamViecSapToi)
        {
            throw new ConflictException("Bac si con ca lam viec sap toi, khong the xoa.");
        }

        entity.TrangThai = TrangThaiBacSi.NghiViec;
        await _db.SaveChangesAsync(cancellationToken);
    }
}
