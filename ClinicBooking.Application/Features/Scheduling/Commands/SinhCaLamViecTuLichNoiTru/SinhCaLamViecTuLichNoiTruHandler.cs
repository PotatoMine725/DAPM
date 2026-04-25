using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Domain.Entities;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.Scheduling.Commands.SinhCaLamViecTuLichNoiTru;

public sealed class SinhCaLamViecTuLichNoiTruHandler : IRequestHandler<SinhCaLamViecTuLichNoiTruCommand, int>
{
    private readonly IAppDbContext _db;

    public SinhCaLamViecTuLichNoiTruHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<int> Handle(SinhCaLamViecTuLichNoiTruCommand request, CancellationToken cancellationToken)
    {
        var homNay = DateOnly.FromDateTime(DateTime.UtcNow);
        var ngayKetThuc = homNay.AddDays(request.SoNgaySinhTruoc);
        var lichs = await _db.LichNoiTru
            .AsNoTracking()
            .Include(x => x.BacSi)
            .Include(x => x.Phong)
            .Include(x => x.DinhNghiaCa)
            .Where(x => x.TrangThai)
            .ToListAsync(cancellationToken);

        var count = 0;
        foreach (var lich in lichs)
        {
            for (var ngay = homNay; ngay <= ngayKetThuc; ngay = ngay.AddDays(1))
            {
                if ((int)ngay.DayOfWeek != lich.NgayTrongTuan) continue;
                var exists = await _db.CaLamViec.AnyAsync(x => x.IdBacSi == lich.IdBacSi && x.NgayLamViec == ngay, cancellationToken);
                if (exists) continue;

                var entity = new CaLamViec
                {
                    IdBacSi = lich.IdBacSi,
                    IdPhong = lich.IdPhong,
                    IdChuyenKhoa = lich.BacSi.IdChuyenKhoa,
                    IdDinhNghiaCa = lich.IdDinhNghiaCa,
                    NgayLamViec = ngay,
                    GioBatDau = lich.DinhNghiaCa.GioBatDauMacDinh,
                    GioKetThuc = lich.DinhNghiaCa.GioKetThucMacDinh,
                    ThoiGianSlot = 15,
                    SoSlotToiDa = 1,
                    SoSlotDaDat = 0,
                    TrangThaiDuyet = TrangThaiDuyetCa.DaDuyet,
                    NguonTaoCa = NguonTaoCa.TuDong,
                    NgayTao = DateTime.UtcNow
                };
                _db.CaLamViec.Add(entity);
                count++;
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
        return count;
    }
}
