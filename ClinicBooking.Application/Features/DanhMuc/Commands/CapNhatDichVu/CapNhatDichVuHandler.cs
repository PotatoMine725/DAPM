using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.DanhMuc.Commands.CapNhatDichVu;

public sealed class CapNhatDichVuHandler : IRequestHandler<CapNhatDichVuCommand, Unit>
{
    private readonly IAppDbContext _db;

    public CapNhatDichVuHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Unit> Handle(CapNhatDichVuCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.DichVu
            .FirstOrDefaultAsync(x => x.IdDichVu == request.IdDichVu, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay dich vu.");

        var chuyenKhoaTonTai = await _db.ChuyenKhoa
            .AnyAsync(x => x.IdChuyenKhoa == request.IdChuyenKhoa, cancellationToken);
        if (!chuyenKhoaTonTai)
        {
            throw new NotFoundException("Khong tim thay chuyen khoa.");
        }

        var tenDaTonTai = await _db.DichVu
            .AnyAsync(
                x => x.IdDichVu != request.IdDichVu
                     && x.IdChuyenKhoa == request.IdChuyenKhoa
                     && x.TenDichVu == request.TenDichVu,
                cancellationToken);
        if (tenDaTonTai)
        {
            throw new ConflictException("Ten dich vu da ton tai trong chuyen khoa nay.");
        }

        entity.IdChuyenKhoa = request.IdChuyenKhoa;
        entity.TenDichVu = request.TenDichVu;
        entity.MoTa = request.MoTa;
        entity.ThoiGianUocTinh = request.ThoiGianUocTinh;
        entity.HienThi = request.HienThi;

        await _db.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
