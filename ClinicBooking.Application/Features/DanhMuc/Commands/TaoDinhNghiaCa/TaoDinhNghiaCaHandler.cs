using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.DanhMuc.Commands.TaoDinhNghiaCa;

public sealed class TaoDinhNghiaCaHandler : IRequestHandler<TaoDinhNghiaCaCommand, int>
{
    private readonly IAppDbContext _db;

    public TaoDinhNghiaCaHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<int> Handle(TaoDinhNghiaCaCommand request, CancellationToken cancellationToken)
    {
        var tenCaDaTonTai = await _db.DinhNghiaCa
            .AnyAsync(x => x.TenCa == request.TenCa, cancellationToken);
        if (tenCaDaTonTai)
        {
            throw new ConflictException("Ten ca da ton tai.");
        }

        var entity = new DinhNghiaCa
        {
            TenCa = request.TenCa,
            GioBatDauMacDinh = request.GioBatDauMacDinh,
            GioKetThucMacDinh = request.GioKetThucMacDinh,
            MoTa = request.MoTa,
            TrangThai = request.TrangThai
        };

        _db.DinhNghiaCa.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return entity.IdDinhNghiaCa;
    }
}
