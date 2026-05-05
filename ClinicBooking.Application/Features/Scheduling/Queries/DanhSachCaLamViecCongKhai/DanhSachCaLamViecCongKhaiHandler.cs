using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Features.Scheduling.Dtos;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.Scheduling.Queries.DanhSachCaLamViecCongKhai;

public sealed class DanhSachCaLamViecCongKhaiHandler : IRequestHandler<DanhSachCaLamViecCongKhaiQuery, IReadOnlyList<CaLamViecPublicResponse>>
{
    private readonly IAppDbContext _db;

    public DanhSachCaLamViecCongKhaiHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<CaLamViecPublicResponse>> Handle(DanhSachCaLamViecCongKhaiQuery request, CancellationToken cancellationToken)
    {
        var query = _db.CaLamViec
            .AsNoTracking()
            .Include(x => x.BacSi)
            .Include(x => x.Phong)
            .Include(x => x.ChuyenKhoa)
            .AsQueryable();

        if (request.IdBacSi.HasValue)
        {
            query = query.Where(x => x.IdBacSi == request.IdBacSi.Value);
        }

        if (request.IdChuyenKhoa.HasValue)
        {
            query = query.Where(x => x.IdChuyenKhoa == request.IdChuyenKhoa.Value);
        }

        if (request.IdPhong.HasValue)
        {
            query = query.Where(x => x.IdPhong == request.IdPhong.Value);
        }

        if (request.TuNgay.HasValue)
        {
            query = query.Where(x => x.NgayLamViec >= request.TuNgay.Value);
        }

        if (request.DenNgay.HasValue)
        {
            query = query.Where(x => x.NgayLamViec <= request.DenNgay.Value);
        }

        if (request.ConTrong is not null)
        {
            query = query.Where(x => (x.SoSlotDaDat < x.SoSlotToiDa) == request.ConTrong.Value);
        }

        query = query.Where(x => x.TrangThaiDuyet == TrangThaiDuyetCa.DaDuyet);

        return await query
            .OrderBy(x => x.NgayLamViec)
            .ThenBy(x => x.GioBatDau)
            .Skip((request.SoTrang - 1) * request.KichThuocTrang)
            .Take(request.KichThuocTrang)
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
            .ToListAsync(cancellationToken);
    }
}
