using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.NghiPhep.Commands.TuChoiDonNghiPhep;

public sealed class TuChoiDonNghiPhepHandler : IRequestHandler<TuChoiDonNghiPhepCommand>
{
    private readonly IAppDbContext _db;

    public TuChoiDonNghiPhepHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task Handle(TuChoiDonNghiPhepCommand request, CancellationToken cancellationToken)
    {
        var don = await _db.DonNghiPhep.FirstOrDefaultAsync(x => x.IdDonNghiPhep == request.IdDonNghiPhep, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay don nghi phep.");

        don.TrangThaiDuyet = TrangThaiDuyetDon.TuChoi;
        don.IdNguoiDuyet = request.IdNguoiDuyet;
        don.LyDoTuChoi = request.LyDoTuChoi;
        don.NgayXuLy = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
    }
}
