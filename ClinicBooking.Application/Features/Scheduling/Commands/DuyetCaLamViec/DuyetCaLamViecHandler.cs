using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Abstractions.Scheduling;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.Scheduling.Commands.DuyetCaLamViec;

public sealed class DuyetCaLamViecHandler : IRequestHandler<DuyetCaLamViecCommand>
{
    private readonly IAppDbContext _db;
    private readonly ICaLamViecConflictChecker _conflictChecker;

    public DuyetCaLamViecHandler(IAppDbContext db, ICaLamViecConflictChecker conflictChecker)
    {
        _db = db;
        _conflictChecker = conflictChecker;
    }

    public async Task Handle(DuyetCaLamViecCommand request, CancellationToken cancellationToken)
    {
        var caLamViec = await _db.CaLamViec.FirstOrDefaultAsync(x => x.IdCaLamViec == request.IdCaLamViec, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay ca lam viec.");

        if (request.ChapNhan)
        {
            await _conflictChecker.EnsureKhongXungDotAsync(
                caLamViec.IdBacSi,
                caLamViec.IdPhong,
                caLamViec.NgayLamViec,
                caLamViec.GioBatDau,
                caLamViec.GioKetThuc,
                caLamViec.IdCaLamViec,
                cancellationToken);
        }

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
