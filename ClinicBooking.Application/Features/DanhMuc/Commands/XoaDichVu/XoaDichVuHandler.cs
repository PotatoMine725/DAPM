using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.DanhMuc.Commands.XoaDichVu;

public sealed class XoaDichVuHandler : IRequestHandler<XoaDichVuCommand, Unit>
{
    private readonly IAppDbContext _db;

    public XoaDichVuHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Unit> Handle(XoaDichVuCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.DichVu
            .FirstOrDefaultAsync(x => x.IdDichVu == request.IdDichVu, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay dich vu.");

        var dangDuocSuDung = await _db.LichHen
            .AnyAsync(x => x.IdDichVu == request.IdDichVu, cancellationToken);

        if (dangDuocSuDung)
        {
            throw new ConflictException("Khong the xoa dich vu dang duoc su dung boi lich hen.");
        }

        _db.DichVu.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
