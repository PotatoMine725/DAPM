using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Abstractions.Scheduling;
using ClinicBooking.Domain.Entities;
using ClinicBooking.Domain.Enums;
using MediatR;

namespace ClinicBooking.Application.Features.Scheduling.Commands.TaoCaLamViec;

public sealed class TaoCaLamViecHandler : IRequestHandler<TaoCaLamViecCommand, int>
{
    private readonly IAppDbContext _db;
    private readonly ICaLamViecConflictChecker _conflictChecker;

    public TaoCaLamViecHandler(IAppDbContext db, ICaLamViecConflictChecker conflictChecker)
    {
        _db = db;
        _conflictChecker = conflictChecker;
    }

    public async Task<int> Handle(TaoCaLamViecCommand request, CancellationToken cancellationToken)
    {
        await _conflictChecker.EnsureKhongXungDotAsync(
            request.IdBacSi,
            request.IdPhong,
            request.NgayLamViec,
            request.GioBatDau,
            request.GioKetThuc,
            null,
            cancellationToken);

        var entity = new CaLamViec
        {
            IdBacSi = request.IdBacSi,
            IdPhong = request.IdPhong,
            IdChuyenKhoa = request.IdChuyenKhoa,
            IdDinhNghiaCa = request.IdDinhNghiaCa,
            NgayLamViec = request.NgayLamViec,
            GioBatDau = request.GioBatDau,
            GioKetThuc = request.GioKetThuc,
            ThoiGianSlot = request.ThoiGianSlot,
            SoSlotToiDa = request.SoSlotToiDa,
            SoSlotDaDat = 0,
            TrangThaiDuyet = TrangThaiDuyetCa.ChoDuyet,
            NguonTaoCa = request.IdBacSiYeuCau.HasValue ? NguonTaoCa.BacSiDangKy : NguonTaoCa.TuDong,
            IdBacSiYeuCau = request.IdBacSiYeuCau,
            NgayTao = DateTime.UtcNow
        };

        _db.CaLamViec.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return entity.IdCaLamViec;
    }
}
