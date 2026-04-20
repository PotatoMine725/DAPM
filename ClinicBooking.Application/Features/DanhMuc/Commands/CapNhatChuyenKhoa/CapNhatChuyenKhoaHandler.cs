using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.DanhMuc.Commands.CapNhatChuyenKhoa;

public sealed class CapNhatChuyenKhoaHandler : IRequestHandler<CapNhatChuyenKhoaCommand, Unit>
{
    private readonly IAppDbContext _db;

    public CapNhatChuyenKhoaHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Unit> Handle(CapNhatChuyenKhoaCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.ChuyenKhoa
            .FirstOrDefaultAsync(x => x.IdChuyenKhoa == request.IdChuyenKhoa, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay chuyen khoa.");

        var tenDaTonTai = await _db.ChuyenKhoa
            .AnyAsync(x => x.TenChuyenKhoa == request.TenChuyenKhoa && x.IdChuyenKhoa != request.IdChuyenKhoa, cancellationToken);
        if (tenDaTonTai)
        {
            throw new ConflictException("Ten chuyen khoa da ton tai.");
        }

        entity.TenChuyenKhoa = request.TenChuyenKhoa;
        entity.MoTa = request.MoTa;
        entity.ThoiGianSlotMacDinh = request.ThoiGianSlotMacDinh;
        entity.GioMoDatLich = request.GioMoDatLich;
        entity.GioDongDatLich = request.GioDongDatLich;
        entity.HienThi = request.HienThi;

        await _db.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
