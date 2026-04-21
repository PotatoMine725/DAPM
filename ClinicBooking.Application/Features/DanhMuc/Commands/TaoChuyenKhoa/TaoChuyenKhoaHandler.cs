using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.DanhMuc.Commands.TaoChuyenKhoa;

public sealed class TaoChuyenKhoaHandler : IRequestHandler<TaoChuyenKhoaCommand, int>
{
    private readonly IAppDbContext _db;

    public TaoChuyenKhoaHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<int> Handle(TaoChuyenKhoaCommand request, CancellationToken cancellationToken)
    {
        var tenDaTonTai = await _db.ChuyenKhoa
            .AnyAsync(x => x.TenChuyenKhoa == request.TenChuyenKhoa, cancellationToken);
        if (tenDaTonTai)
        {
            throw new ConflictException("Ten chuyen khoa da ton tai.");
        }

        var entity = new ChuyenKhoa
        {
            TenChuyenKhoa = request.TenChuyenKhoa,
            MoTa = request.MoTa,
            ThoiGianSlotMacDinh = request.ThoiGianSlotMacDinh,
            GioMoDatLich = request.GioMoDatLich,
            GioDongDatLich = request.GioDongDatLich,
            HienThi = request.HienThi
        };

        _db.ChuyenKhoa.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return entity.IdChuyenKhoa;
    }
}
