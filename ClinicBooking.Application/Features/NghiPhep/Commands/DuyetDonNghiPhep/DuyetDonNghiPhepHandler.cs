using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.NghiPhep.Commands.DuyetDonNghiPhep;

public sealed class DuyetDonNghiPhepHandler : IRequestHandler<DuyetDonNghiPhepCommand>
{
    private readonly IAppDbContext _db;

    public DuyetDonNghiPhepHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task Handle(DuyetDonNghiPhepCommand request, CancellationToken cancellationToken)
    {
        var don = await _db.DonNghiPhep
            .Include(x => x.BacSi)
            .Include(x => x.CaLamViec)
            .FirstOrDefaultAsync(x => x.IdDonNghiPhep == request.IdDonNghiPhep, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay don nghi phep.");

        if (request.ChapNhan)
        {
            var cacCa = await _db.CaLamViec
                .AsNoTracking()
                .Where(x => x.IdBacSi == don.IdBacSi)
                .Where(x => x.NgayLamViec >= don.CaLamViec.NgayLamViec && x.NgayLamViec <= don.CaLamViec.NgayLamViec)
                .Where(x => x.TrangThaiDuyet == TrangThaiDuyetCa.DaDuyet)
                .ToListAsync(cancellationToken);

            var conflicts = cacCa
                .Where(x => x.SoSlotDaDat > 0)
                .Select(x => $"Ca {x.IdCaLamViec} ngay {x.NgayLamViec:yyyy-MM-dd} co {x.SoSlotDaDat} lich hen.")
                .ToList();

            if (conflicts.Count > 0)
            {
                throw new ConflictException("Khong the duyet don nghi phep vi con lich hen dang ton tai: " + string.Join(" ", conflicts));
            }

            foreach (var ca in cacCa)
            {
                if (ca.SoSlotDaDat == 0)
                {
                    ca.TrangThaiDuyet = TrangThaiDuyetCa.DaHuy;
                }
            }

            don.TrangThaiDuyet = TrangThaiDuyetDon.DaDuyet;
        }
        else
        {
            don.TrangThaiDuyet = TrangThaiDuyetDon.TuChoi;
        }

        don.IdNguoiDuyet = request.IdNguoiDuyet;
        don.NgayXuLy = DateTime.UtcNow;
        don.LyDoTuChoi = request.ChapNhan ? null : request.LyDoTuChoi;

        await _db.SaveChangesAsync(cancellationToken);
    }
}