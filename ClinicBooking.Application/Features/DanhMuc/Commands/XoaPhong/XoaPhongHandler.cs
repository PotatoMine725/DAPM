using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.DanhMuc.Commands.XoaPhong;

public sealed class XoaPhongHandler : IRequestHandler<XoaPhongCommand, Unit>
{
    private readonly IAppDbContext _db;

    public XoaPhongHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Unit> Handle(XoaPhongCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Phong
            .FirstOrDefaultAsync(x => x.IdPhong == request.IdPhong, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay phong.");

        var dangDuocSuDung = await _db.CaLamViec
            .AnyAsync(x => x.IdPhong == request.IdPhong, cancellationToken)
            || await _db.LichNoiTru.AnyAsync(x => x.IdPhong == request.IdPhong, cancellationToken);

        if (dangDuocSuDung)
        {
            throw new ConflictException("Khong the xoa phong dang duoc su dung.");
        }

        _db.Phong.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
