using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Features.Scheduling.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.Scheduling.Queries.LayChiTietCaLamViecCongKhai;

public sealed class LayChiTietCaLamViecCongKhaiHandler : IRequestHandler<LayChiTietCaLamViecCongKhaiQuery, CaLamViecPublicResponse?>
{
    private readonly IAppDbContext _db;

    public LayChiTietCaLamViecCongKhaiHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<CaLamViecPublicResponse?> Handle(LayChiTietCaLamViecCongKhaiQuery request, CancellationToken cancellationToken)
    {
        return await _db.CaLamViec
            .AsNoTracking()
            .Include(x => x.BacSi)
            .Include(x => x.Phong)
            .Include(x => x.ChuyenKhoa)
            .Where(x => x.IdCaLamViec == request.IdCaLamViec)
            .Select(x => new CaLamViecPublicResponse(
                x.IdCaLamViec,
                x.IdBacSi,
                x.IdPhong,
                x.IdChuyenKhoa,
                x.IdDinhNghiaCa,
                x.NgayLamViec,
                x.GioBatDau,
                x.GioKetThuc,
                x.ThoiGianSlot,
                x.TrangThaiDuyet.ToString(),
                x.NguonTaoCa.ToString(),
                x.BacSi.HoTen,
                x.Phong.MaPhong,
                x.ChuyenKhoa.TenChuyenKhoa,
                x.SoSlotDaDat < x.SoSlotToiDa))
            .FirstOrDefaultAsync(cancellationToken);
    }
}