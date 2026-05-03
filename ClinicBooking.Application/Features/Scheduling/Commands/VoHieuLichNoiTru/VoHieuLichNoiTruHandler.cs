using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.Scheduling.Commands.VoHieuLichNoiTru;

public sealed class VoHieuLichNoiTruHandler : IRequestHandler<VoHieuLichNoiTruCommand>
{
    private readonly IAppDbContext _db;

    public VoHieuLichNoiTruHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task Handle(VoHieuLichNoiTruCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.LichNoiTru.FirstOrDefaultAsync(x => x.IdLichNoiTru == request.IdLichNoiTru, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay lich noi tru.");

        entity.TrangThai = false;
        await _db.SaveChangesAsync(cancellationToken);
    }
}
