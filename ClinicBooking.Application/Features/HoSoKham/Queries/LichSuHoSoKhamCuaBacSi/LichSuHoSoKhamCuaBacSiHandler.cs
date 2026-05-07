using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.HoSoKham.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.HoSoKham.Queries.LichSuHoSoKhamCuaBacSi;

public class LichSuHoSoKhamCuaBacSiHandler 
    : IRequestHandler<LichSuHoSoKhamCuaBacSiQuery, IReadOnlyList<HoSoKhamTomTatResponse>>
{
    private readonly IAppDbContext _context;

    public LichSuHoSoKhamCuaBacSiHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<HoSoKhamTomTatResponse>> Handle(
        LichSuHoSoKhamCuaBacSiQuery request,
        CancellationToken cancellationToken)
    {
        // Kiểm tra bác sĩ tồn tại
        var bacSiExists = await _context.BacSi.AnyAsync(
            x => x.IdBacSi == request.IdBacSi,
            cancellationToken);

        if (!bacSiExists)
            throw new NotFoundException("Bac si khong ton tai.");

        // Lấy danh sách hồ sơ khám của bác sĩ
        var result = await _context.HoSoKham
            .Where(x => x.IdBacSi == request.IdBacSi)
            .Include(x => x.LichHen)
            .Include(x => x.BacSi)
            .OrderByDescending(x => x.IdHoSoKham)
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

        return result;
    }
}
