using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.BenhNhan.Commands.CapNhatThongTinBenhNhan;

public sealed class CapNhatThongTinBenhNhanHandler : IRequestHandler<CapNhatThongTinBenhNhanCommand, Unit>
{
    private readonly IAppDbContext _db;

    public CapNhatThongTinBenhNhanHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Unit> Handle(CapNhatThongTinBenhNhanCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.BenhNhan
            .FirstOrDefaultAsync(x => x.IdBenhNhan == request.IdBenhNhan, cancellationToken)
            ?? throw new NotFoundException("Không tìm thấy hồ sơ bệnh nhân.");

        var cccdMoi = string.IsNullOrWhiteSpace(request.Cccd) ? null : request.Cccd.Trim();
        if (!string.IsNullOrWhiteSpace(cccdMoi))
        {
            var cccdTrung = await _db.BenhNhan
                .AnyAsync(x => x.IdBenhNhan != entity.IdBenhNhan && x.Cccd == cccdMoi, cancellationToken);
            if (cccdTrung)
                throw new ConflictException("CCCD đã được sử dụng bởi bệnh nhân khác.");
        }

        entity.HoTen = request.HoTen.Trim();
        entity.NgaySinh = request.NgaySinh;
        entity.GioiTinh = request.GioiTinh;
        entity.Cccd = cccdMoi;
        entity.DiaChi = string.IsNullOrWhiteSpace(request.DiaChi) ? null : request.DiaChi.Trim();

        await _db.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
