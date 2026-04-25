using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.Scheduling.Commands.TaoLichNoiTru;

public sealed class TaoLichNoiTruHandler : IRequestHandler<TaoLichNoiTruCommand, int>
{
    private readonly IAppDbContext _db;

    public TaoLichNoiTruHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<int> Handle(TaoLichNoiTruCommand request, CancellationToken cancellationToken)
    {
        var bacSi = await _db.BacSi.FirstOrDefaultAsync(x => x.IdBacSi == request.IdBacSi, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay bac si.");

        var phongTonTai = await _db.Phong.AnyAsync(x => x.IdPhong == request.IdPhong, cancellationToken);
        if (!phongTonTai)
        {
            throw new NotFoundException("Khong tim thay phong.");
        }

        var caTonTai = await _db.DinhNghiaCa.AnyAsync(x => x.IdDinhNghiaCa == request.IdDinhNghiaCa, cancellationToken);
        if (!caTonTai)
        {
            throw new NotFoundException("Khong tim thay dinh nghia ca.");
        }

        var entity = new LichNoiTru
        {
            IdBacSi = request.IdBacSi,
            IdPhong = request.IdPhong,
            IdDinhNghiaCa = request.IdDinhNghiaCa,
            NgayTrongTuan = request.NgayTrongTuan,
            NgayApDung = request.NgayApDung,
            NgayKetThuc = request.NgayKetThuc,
            TrangThai = true
        };

        _db.LichNoiTru.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return entity.IdLichNoiTru;
    }
}
