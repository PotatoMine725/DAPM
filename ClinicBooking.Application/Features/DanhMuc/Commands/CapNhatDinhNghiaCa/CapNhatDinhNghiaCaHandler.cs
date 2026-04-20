using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.DanhMuc.Commands.CapNhatDinhNghiaCa;

public sealed class CapNhatDinhNghiaCaHandler : IRequestHandler<CapNhatDinhNghiaCaCommand, Unit>
{
    private readonly IAppDbContext _db;

    public CapNhatDinhNghiaCaHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Unit> Handle(CapNhatDinhNghiaCaCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.DinhNghiaCa
            .FirstOrDefaultAsync(x => x.IdDinhNghiaCa == request.IdDinhNghiaCa, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay dinh nghia ca.");

        var tenCaDaTonTai = await _db.DinhNghiaCa
            .AnyAsync(x => x.IdDinhNghiaCa != request.IdDinhNghiaCa && x.TenCa == request.TenCa, cancellationToken);
        if (tenCaDaTonTai)
        {
            throw new ConflictException("Ten ca da ton tai.");
        }

        entity.TenCa = request.TenCa;
        entity.GioBatDauMacDinh = request.GioBatDauMacDinh;
        entity.GioKetThucMacDinh = request.GioKetThucMacDinh;
        entity.MoTa = request.MoTa;
        entity.TrangThai = request.TrangThai;

        await _db.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
