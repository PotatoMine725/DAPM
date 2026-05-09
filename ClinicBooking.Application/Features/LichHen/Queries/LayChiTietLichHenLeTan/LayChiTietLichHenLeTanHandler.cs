using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.LichHen.Queries.LayChiTietLichHenLeTan;

public class LayChiTietLichHenLeTanHandler
    : IRequestHandler<LayChiTietLichHenLeTanQuery, ChiTietLichHenLeTanResponse>
{
    private readonly IAppDbContext _db;

    public LayChiTietLichHenLeTanHandler(IAppDbContext db) => _db = db;

    public async Task<ChiTietLichHenLeTanResponse> Handle(
        LayChiTietLichHenLeTanQuery request,
        CancellationToken cancellationToken)
    {
        var lichHen = await _db.LichHen
            .AsNoTracking()
            .Include(x => x.BenhNhan).ThenInclude(bn => bn.TaiKhoan)
            .Include(x => x.CaLamViec).ThenInclude(ca => ca.BacSi)
            .Include(x => x.DichVu).ThenInclude(dv => dv.ChuyenKhoa)
            .Include(x => x.HangCho)
            .FirstOrDefaultAsync(x => x.IdLichHen == request.IdLichHen, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay lich hen.");

        return new ChiTietLichHenLeTanResponse(
            lichHen.IdLichHen,
            lichHen.MaLichHen,
            lichHen.BenhNhan.HoTen,
            lichHen.BenhNhan.TaiKhoan.SoDienThoai,
            lichHen.DichVu.TenDichVu,
            lichHen.DichVu.ChuyenKhoa.TenChuyenKhoa,
            lichHen.DichVu.IdChuyenKhoa,
            lichHen.CaLamViec.NgayLamViec,
            lichHen.CaLamViec.GioBatDau,
            lichHen.CaLamViec.GioKetThuc,
            lichHen.TrangThai,
            lichHen.HinhThucDat,
            lichHen.TrieuChung,
            lichHen.BacSiMongMuonNote,
            lichHen.IdBacSiMongMuon,
            lichHen.CaLamViec.BacSi?.HoTen,
            lichHen.HangCho != null
        );
    }
}
