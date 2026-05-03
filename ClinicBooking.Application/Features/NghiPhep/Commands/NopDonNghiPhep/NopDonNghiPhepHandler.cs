using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Domain.Entities;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.NghiPhep.Commands.NopDonNghiPhep;

public sealed class NopDonNghiPhepHandler : IRequestHandler<NopDonNghiPhepCommand, int>
{
    private readonly IAppDbContext _db;

    public NopDonNghiPhepHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<int> Handle(NopDonNghiPhepCommand request, CancellationToken cancellationToken)
    {
        var caLamViec = await _db.CaLamViec.FirstOrDefaultAsync(x => x.IdCaLamViec == request.IdCaLamViec, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay ca lam viec.");

        var entity = new DonNghiPhep
        {
            IdBacSi = request.IdBacSi,
            IdCaLamViec = request.IdCaLamViec,
            LoaiNghiPhep = Enum.Parse<LoaiNghiPhep>(request.LoaiNghiPhep, true),
            LyDo = request.LyDo,
            TrangThaiDuyet = TrangThaiDuyetDon.ChoDuyet,
            NgayGuiDon = DateTime.UtcNow
        };

        _db.DonNghiPhep.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return entity.IdDonNghiPhep;
    }
}
