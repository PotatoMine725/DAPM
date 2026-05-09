using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Common;
using ClinicBooking.Application.Features.LichHen.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.LichHen.Queries.DanhSachTatCaLichHen;

public class DanhSachTatCaLichHenHandler
    : IRequestHandler<DanhSachTatCaLichHenQuery, PhanTrangKetQua<LichHenTomTatResponse>>
{
    private readonly IAppDbContext _db;

    public DanhSachTatCaLichHenHandler(IAppDbContext db) => _db = db;

    public async Task<PhanTrangKetQua<LichHenTomTatResponse>> Handle(
        DanhSachTatCaLichHenQuery request,
        CancellationToken cancellationToken)
    {
        var query = _db.LichHen.AsNoTracking();

        if (request.Ngay.HasValue)
            query = query.Where(x => x.CaLamViec.NgayLamViec == request.Ngay.Value);

        if (request.TrangThai.HasValue)
            query = query.Where(x => x.TrangThai == request.TrangThai.Value);

        var tongSo = await query.CountAsync(cancellationToken);

        var trang = Math.Max(1, request.Trang);
        var danhSach = await query
            .OrderByDescending(x => x.CaLamViec.NgayLamViec)
            .ThenBy(x => x.CaLamViec.GioBatDau)
            .ThenBy(x => x.SoSlot)
            .Skip((trang - 1) * request.SoTrenMoiTrang)
            .Take(request.SoTrenMoiTrang)
            .Select(x => new LichHenTomTatResponse(
                x.IdLichHen,
                x.MaLichHen,
                x.IdBenhNhan,
                x.BenhNhan.HoTen,
                x.IdCaLamViec,
                x.CaLamViec.NgayLamViec,
                x.CaLamViec.GioBatDau,
                x.SoSlot,
                x.DichVu.TenDichVu,
                x.TrangThai,
                x.NgayTao,
                x.HangCho != null))
            .ToListAsync(cancellationToken);

        return new PhanTrangKetQua<LichHenTomTatResponse>(danhSach, tongSo, trang, request.SoTrenMoiTrang);
    }
}
