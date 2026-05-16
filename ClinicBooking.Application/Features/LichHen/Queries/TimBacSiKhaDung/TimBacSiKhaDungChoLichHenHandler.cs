using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.LichHen.Queries.TimBacSiKhaDung;

public class TimBacSiKhaDungChoLichHenHandler
    : IRequestHandler<TimBacSiKhaDungChoLichHenQuery, List<BacSiKhaDungItem>>
{
    private readonly IAppDbContext _db;

    public TimBacSiKhaDungChoLichHenHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<List<BacSiKhaDungItem>> Handle(
        TimBacSiKhaDungChoLichHenQuery request,
        CancellationToken cancellationToken)
    {
        // Load lich hen cung ca lam viec va dich vu de lay IdChuyenKhoa va NgayLamViec
        var lichHen = await _db.LichHen
            .AsNoTracking()
            .Include(x => x.CaLamViec)
            .Include(x => x.DichVu)
            .FirstOrDefaultAsync(x => x.IdLichHen == request.IdLichHen, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay lich hen.");

        var idChuyenKhoa = lichHen.DichVu.IdChuyenKhoa;
        var ngayLamViec = lichHen.CaLamViec.NgayLamViec;
        var idCaLamViecHienTai = lichHen.IdCaLamViec;

        var danhSachCa = await _db.CaLamViec
            .AsNoTracking()
            .Include(c => c.BacSi)
            .Where(c =>
                c.NgayLamViec == ngayLamViec
                && c.IdChuyenKhoa == idChuyenKhoa
                && c.TrangThaiDuyet == TrangThaiDuyetCa.DaDuyet
                && c.SoSlotDaDat < c.SoSlotToiDa
                && c.IdCaLamViec != idCaLamViecHienTai
                && c.BacSi.TrangThai == TrangThaiBacSi.DangLam
                && c.BacSi.HoTen.Contains(request.TenBacSi))
            .OrderByDescending(c => c.SoSlotToiDa - c.SoSlotDaDat)
            .Take(10)
            .Select(c => new BacSiKhaDungItem(
                c.BacSi.IdBacSi,
                c.BacSi.HoTen,
                c.SoSlotToiDa - c.SoSlotDaDat,
                c.IdCaLamViec))
            .ToListAsync(cancellationToken);

        return danhSachCa;
    }
}
