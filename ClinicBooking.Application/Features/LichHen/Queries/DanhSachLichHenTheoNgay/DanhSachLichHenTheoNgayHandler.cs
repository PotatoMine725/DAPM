using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Features.LichHen.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.LichHen.Queries.DanhSachLichHenTheoNgay;

public class DanhSachLichHenTheoNgayHandler
    : IRequestHandler<DanhSachLichHenTheoNgayQuery, IReadOnlyList<LichHenTomTatResponse>>
{
    private readonly IAppDbContext _db;

    public DanhSachLichHenTheoNgayHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<LichHenTomTatResponse>> Handle(
        DanhSachLichHenTheoNgayQuery request,
        CancellationToken cancellationToken)
    {
        return await _db.LichHen
            .AsNoTracking()
            .Where(x => x.CaLamViec.NgayLamViec == request.Ngay)
            .OrderBy(x => x.CaLamViec.GioBatDau)
            .ThenBy(x => x.SoSlot)
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
                x.NgayTao))
            .ToListAsync(cancellationToken);
    }
}
