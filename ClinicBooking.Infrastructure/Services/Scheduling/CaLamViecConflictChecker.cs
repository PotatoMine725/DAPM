using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Abstractions.Scheduling;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Infrastructure.Services.Scheduling;

public sealed class CaLamViecConflictChecker : ICaLamViecConflictChecker
{
    private readonly IAppDbContext _db;

    public CaLamViecConflictChecker(IAppDbContext db)
    {
        _db = db;
    }

    public async Task EnsureKhongXungDotAsync(
        int idBacSi,
        int idPhong,
        DateOnly ngayLamViec,
        TimeOnly gioBatDau,
        TimeOnly gioKetThuc,
        int? idCaLamViecBoQua,
        CancellationToken cancellationToken = default)
    {
        var messages = new List<string>();

        var bacSiConflict = await _db.CaLamViec
            .AsNoTracking()
            .Include(x => x.BacSi)
            .Where(x => x.IdBacSi == idBacSi)
            .Where(x => x.NgayLamViec == ngayLamViec)
            .Where(x => x.TrangThaiDuyet != TrangThaiDuyetCa.DaHuy)
            .Where(x => !idCaLamViecBoQua.HasValue || x.IdCaLamViec != idCaLamViecBoQua.Value)
            .Where(x => x.GioBatDau < gioKetThuc && x.GioKetThuc > gioBatDau)
            .Select(x => new { x.BacSi.HoTen, x.GioBatDau, x.GioKetThuc })
            .FirstOrDefaultAsync(cancellationToken);
        if (bacSiConflict is not null)
        {
            messages.Add($"Bác sĩ {bacSiConflict.HoTen} đã có ca khác lúc {bacSiConflict.GioBatDau:HH\\:mm}-{bacSiConflict.GioKetThuc:HH\\:mm} ngày {ngayLamViec:dd/MM/yyyy}.");
        }

        var phongConflict = await _db.CaLamViec
            .AsNoTracking()
            .Include(x => x.Phong)
            .Where(x => x.IdPhong == idPhong)
            .Where(x => x.NgayLamViec == ngayLamViec)
            .Where(x => x.TrangThaiDuyet != TrangThaiDuyetCa.DaHuy)
            .Where(x => !idCaLamViecBoQua.HasValue || x.IdCaLamViec != idCaLamViecBoQua.Value)
            .Where(x => x.GioBatDau < gioKetThuc && x.GioKetThuc > gioBatDau)
            .Select(x => new { x.Phong.TenPhong, x.GioBatDau, x.GioKetThuc })
            .FirstOrDefaultAsync(cancellationToken);
        if (phongConflict is not null)
        {
            messages.Add($"Phòng {phongConflict.TenPhong} đã được đặt lúc {phongConflict.GioBatDau:HH\\:mm}-{phongConflict.GioKetThuc:HH\\:mm} ngày {ngayLamViec:dd/MM/yyyy}.");
        }

        var nghiPhepConflict = await _db.DonNghiPhep
            .AsNoTracking()
            .Include(x => x.BacSi)
            .Where(x => x.IdBacSi == idBacSi)
            .Where(x => x.TrangThaiDuyet == TrangThaiDuyetDon.DaDuyet)
            .Where(x => x.CaLamViec.NgayLamViec <= ngayLamViec && x.CaLamViec.NgayLamViec >= ngayLamViec)
            .Select(x => new { x.BacSi.HoTen, x.CaLamViec.NgayLamViec, x.CaLamViec.GioBatDau, x.CaLamViec.GioKetThuc })
            .FirstOrDefaultAsync(cancellationToken);
        if (nghiPhepConflict is not null)
        {
            messages.Add($"Bác sĩ {nghiPhepConflict.HoTen} đang nghỉ phép từ {nghiPhepConflict.NgayLamViec:dd/MM/yyyy} trong khung giờ {nghiPhepConflict.GioBatDau:HH\\:mm}-{nghiPhepConflict.GioKetThuc:HH\\:mm}.");
        }

        if (messages.Count > 0)
        {
            throw new ConflictException(string.Join(" ", messages));
        }
    }
}
