using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.DanhMuc.Commands.TaoPhong;

public sealed class TaoPhongHandler : IRequestHandler<TaoPhongCommand, int>
{
    private readonly IAppDbContext _db;

    public TaoPhongHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<int> Handle(TaoPhongCommand request, CancellationToken cancellationToken)
    {
        var maPhongDaTonTai = await _db.Phong
            .AnyAsync(x => x.MaPhong == request.MaPhong, cancellationToken);
        if (maPhongDaTonTai)
        {
            throw new ConflictException("Ma phong da ton tai.");
        }

        var entity = new Phong
        {
            MaPhong = request.MaPhong,
            TenPhong = request.TenPhong,
            SucChua = request.SucChua,
            TrangBi = request.TrangBi,
            TrangThai = request.TrangThai
        };

        _db.Phong.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return entity.IdPhong;
    }
}
