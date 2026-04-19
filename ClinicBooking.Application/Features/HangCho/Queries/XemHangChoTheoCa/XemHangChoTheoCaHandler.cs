using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Features.HangCho.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.HangCho.Queries.XemHangChoTheoCa;

public class XemHangChoTheoCaHandler
    : IRequestHandler<XemHangChoTheoCaQuery, IReadOnlyList<HangChoResponse>>
{
    private readonly IAppDbContext _db;

    public XemHangChoTheoCaHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<HangChoResponse>> Handle(
        XemHangChoTheoCaQuery request,
        CancellationToken cancellationToken)
    {
        return await _db.HangCho
            .AsNoTracking()
            .Where(x => x.IdCaLamViec == request.IdCaLamViec)
            .OrderBy(x => x.SoThuTu)
            .Select(x => new HangChoResponse(
                x.IdHangCho,
                x.IdCaLamViec,
                x.IdLichHen,
                x.LichHen.MaLichHen,
                x.LichHen.IdBenhNhan,
                x.LichHen.BenhNhan.HoTen,
                x.SoThuTu,
                x.TrangThai,
                x.NgayCheckIn))
            .ToListAsync(cancellationToken);
    }
}
