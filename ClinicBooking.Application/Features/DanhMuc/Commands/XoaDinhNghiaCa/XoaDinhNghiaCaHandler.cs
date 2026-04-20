using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.DanhMuc.Commands.XoaDinhNghiaCa;

public sealed class XoaDinhNghiaCaHandler : IRequestHandler<XoaDinhNghiaCaCommand, Unit>
{
    private readonly IAppDbContext _db;

    public XoaDinhNghiaCaHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Unit> Handle(XoaDinhNghiaCaCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.DinhNghiaCa
            .FirstOrDefaultAsync(x => x.IdDinhNghiaCa == request.IdDinhNghiaCa, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay dinh nghia ca.");

        var dangDuocSuDung = await _db.CaLamViec
            .AnyAsync(x => x.IdDinhNghiaCa == request.IdDinhNghiaCa, cancellationToken)
            || await _db.LichNoiTru.AnyAsync(x => x.IdDinhNghiaCa == request.IdDinhNghiaCa, cancellationToken);

        if (dangDuocSuDung)
        {
            throw new ConflictException("Khong the xoa dinh nghia ca dang duoc su dung.");
        }

        _db.DinhNghiaCa.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
