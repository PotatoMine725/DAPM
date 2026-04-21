using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.Thuoc.Commands.TaoThuoc;

public sealed class TaoThuocHandler : IRequestHandler<TaoThuocCommand, int>
{
    private readonly IAppDbContext _db;

    public TaoThuocHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<int> Handle(TaoThuocCommand request, CancellationToken cancellationToken)
    {
        var tenDaTonTai = await _db.Thuoc
            .AnyAsync(x => x.TenThuoc == request.TenThuoc, cancellationToken);
        if (tenDaTonTai)
        {
            throw new ConflictException("Ten thuoc da ton tai.");
        }

        var entity = new ClinicBooking.Domain.Entities.Thuoc
        {
            TenThuoc = request.TenThuoc,
            HoatChat = request.HoatChat,
            DonVi = request.DonVi,
            GhiChu = request.GhiChu
        };

        _db.Thuoc.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return entity.IdThuoc;
    }
}
