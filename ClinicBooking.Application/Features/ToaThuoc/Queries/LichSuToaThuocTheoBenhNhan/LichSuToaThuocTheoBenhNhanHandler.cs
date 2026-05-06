using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.ToaThuoc.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.ToaThuoc.Queries.LichSuToaThuocTheoBenhNhan;

public class LichSuToaThuocTheoBenhNhanHandler 
    : IRequestHandler<LichSuToaThuocTheoBenhNhanQuery, IReadOnlyList<ToaThuocResponse>>
{
    private readonly IAppDbContext _context;

    public LichSuToaThuocTheoBenhNhanHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<ToaThuocResponse>> Handle(
        LichSuToaThuocTheoBenhNhanQuery request,
        CancellationToken cancellationToken)
    {
        // Kiểm tra bệnh nhân tồn tại
        var benhNhanExists = await _context.BenhNhan.AnyAsync(
            x => x.IdBenhNhan == request.IdBenhNhan,
            cancellationToken);

        if (!benhNhanExists)
            throw new NotFoundException("Benh nhan khong ton tai.");

        // Lấy danh sách đơn thuốc của bệnh nhân qua HoSoKham
        var result = await _context.ToaThuoc
            .Include(x => x.Thuoc)
            .Include(x => x.HoSoKham)
                .ThenInclude(h => h.LichHen)
            .Where(x => x.HoSoKham.LichHen.IdBenhNhan == request.IdBenhNhan)
            .OrderByDescending(x => x.IdToaThuoc)
            .Skip((request.SoTrang - 1) * request.KichThuocTrang)
            .Take(request.KichThuocTrang)
            .Select(x => new ToaThuocResponse(
                x.IdToaThuoc,
                x.IdHoSoKham,
                x.IdThuoc,
                x.Thuoc.TenThuoc,
                x.LieuLuong,
                x.CachDung,
                x.SoNgayDung,
                x.GhiChu,
                x.HoSoKham.NgayKham,
                x.HoSoKham.LichHen.MaLichHen))
            .ToListAsync(cancellationToken);

        return result;
    }
}
