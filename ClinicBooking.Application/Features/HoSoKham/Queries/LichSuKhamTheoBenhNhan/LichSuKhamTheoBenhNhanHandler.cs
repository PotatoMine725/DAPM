using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.HoSoKham.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.HoSoKham.Queries.LichSuKhamTheoBenhNhan;

public sealed class LichSuKhamTheoBenhNhanHandler
    : IRequestHandler<LichSuKhamTheoBenhNhanQuery, IReadOnlyList<HoSoKhamTomTatResponse>>
{
    private readonly IAppDbContext _db;

    public LichSuKhamTheoBenhNhanHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<HoSoKhamTomTatResponse>> Handle(
        LichSuKhamTheoBenhNhanQuery request,
        CancellationToken cancellationToken)
    {
        var benhNhanTonTai = await _db.BenhNhan
            .AnyAsync(x => x.IdBenhNhan == request.IdBenhNhan, cancellationToken);
        if (!benhNhanTonTai)
        {
            throw new NotFoundException("Khong tim thay benh nhan.");
        }

        return await _db.HoSoKham
            .AsNoTracking()
            .Include(x => x.BacSi)
            .Include(x => x.LichHen)
            .Where(x => x.LichHen.IdBenhNhan == request.IdBenhNhan)
            .OrderByDescending(x => x.NgayKham)
            .Skip((request.SoTrang - 1) * request.KichThuocTrang)
            .Take(request.KichThuocTrang)
            .Select(x => new HoSoKhamTomTatResponse(
                x.IdHoSoKham,
                x.IdLichHen,
                x.LichHen.MaLichHen,
                x.IdBacSi,
                x.BacSi.HoTen,
                x.NgayKham,
                x.ChanDoan,
                x.KetQuaKham,
                x.NgayTao))
            .ToListAsync(cancellationToken);
    }
}
