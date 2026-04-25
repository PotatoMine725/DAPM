using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.Scheduling.Commands.DuyetCaLamViec;

public sealed class DuyetCaLamViecHandler : IRequestHandler<DuyetCaLamViecCommand>
{
    private readonly IAppDbContext _db;

    public DuyetCaLamViecHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task Handle(DuyetCaLamViecCommand request, CancellationToken cancellationToken)
    {
        var caLamViec = await _db.CaLamViec.FirstOrDefaultAsync(x => x.IdCaLamViec == request.IdCaLamViec, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay ca lam viec.");

        caLamViec.TrangThaiDuyet = request.ChapNhan ? TrangThaiDuyetCa.DaDuyet : TrangThaiDuyetCa.DaHuy;
        caLamViec.IdAdminDuyet = request.IdAdminDuyet;
        caLamViec.NgayDuyet = DateTime.UtcNow;
        caLamViec.LyDoTuChoi = request.ChapNhan ? null : request.LyDoTuChoi;
        if (request.ChapNhan)
        {
            caLamViec.IdBacSiYeuCau ??= caLamViec.IdBacSi;
        }

        await _db.SaveChangesAsync(cancellationToken);
    }
}
