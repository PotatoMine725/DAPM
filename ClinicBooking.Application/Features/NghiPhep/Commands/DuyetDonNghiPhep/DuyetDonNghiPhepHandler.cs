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
            .Include(x => x.CaLamViec)
            .FirstOrDefaultAsync(x => x.IdDonNghiPhep == request.IdDonNghiPhep, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay don nghi phep.");

        don.TrangThaiDuyet = TrangThaiDuyetDon.DaDuyet;
        don.IdNguoiDuyet = request.IdNguoiDuyet;
        don.NgayXuLy = DateTime.UtcNow;
        don.CaLamViec.TrangThaiDuyet = TrangThaiDuyetCa.DaHuy;

        await _db.SaveChangesAsync(cancellationToken);
    }
}
