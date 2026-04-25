using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.Scheduling.Commands.CapNhatLichNoiTru;

public sealed class CapNhatLichNoiTruHandler : IRequestHandler<CapNhatLichNoiTruCommand>
{
    private readonly IAppDbContext _db;

    public CapNhatLichNoiTruHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task Handle(CapNhatLichNoiTruCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.LichNoiTru.FirstOrDefaultAsync(x => x.IdLichNoiTru == request.IdLichNoiTru, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay lich noi tru.");

        entity.IdPhong = request.IdPhong;
        entity.IdDinhNghiaCa = request.IdDinhNghiaCa;
        entity.NgayTrongTuan = request.NgayTrongTuan;
        entity.NgayApDung = request.NgayApDung;
        entity.NgayKetThuc = request.NgayKetThuc;

        await _db.SaveChangesAsync(cancellationToken);
    }
}
