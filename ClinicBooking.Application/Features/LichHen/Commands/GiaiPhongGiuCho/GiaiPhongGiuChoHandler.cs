using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.LichHen.Commands.GiaiPhongGiuCho;

public class GiaiPhongGiuChoHandler : IRequestHandler<GiaiPhongGiuChoCommand, Unit>
{
    private readonly IAppDbContext _db;
    private readonly IDateTimeProvider _dateTimeProvider;

    public GiaiPhongGiuChoHandler(IAppDbContext db, IDateTimeProvider dateTimeProvider)
    {
        _db = db;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Unit> Handle(GiaiPhongGiuChoCommand request, CancellationToken cancellationToken)
    {
        var giuCho = await _db.GiuCho
            .FirstOrDefaultAsync(x => x.IdGiuCho == request.IdGiuCho, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay ban ghi giu cho.");

        if (giuCho.DaGiaiPhong)
        {
            throw new ConflictException("Ban ghi giu cho da duoc giai phong truoc do.");
        }

        giuCho.DaGiaiPhong = true;
        giuCho.GioHetHan = _dateTimeProvider.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
