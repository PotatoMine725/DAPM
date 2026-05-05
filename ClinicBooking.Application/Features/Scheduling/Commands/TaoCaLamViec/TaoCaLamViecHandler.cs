using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Domain.Entities;
using ClinicBooking.Domain.Enums;
using MediatR;

namespace ClinicBooking.Application.Features.Scheduling.Commands.TaoCaLamViec;

public sealed class TaoCaLamViecHandler : IRequestHandler<TaoCaLamViecCommand, int>
{
    private readonly IAppDbContext _db;

    public TaoCaLamViecHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<int> Handle(TaoCaLamViecCommand request, CancellationToken cancellationToken)
    {
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
