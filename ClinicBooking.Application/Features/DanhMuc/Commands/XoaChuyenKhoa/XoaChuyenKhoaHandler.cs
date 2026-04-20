using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.DanhMuc.Commands.XoaChuyenKhoa;

public sealed class XoaChuyenKhoaHandler : IRequestHandler<XoaChuyenKhoaCommand, Unit>
{
    private readonly IAppDbContext _db;

    public XoaChuyenKhoaHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Unit> Handle(XoaChuyenKhoaCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.ChuyenKhoa
            .FirstOrDefaultAsync(x => x.IdChuyenKhoa == request.IdChuyenKhoa, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay chuyen khoa.");

        var dangDuocSuDung = await _db.BacSi.AnyAsync(x => x.IdChuyenKhoa == request.IdChuyenKhoa, cancellationToken)
            || await _db.DichVu.AnyAsync(x => x.IdChuyenKhoa == request.IdChuyenKhoa, cancellationToken)
            || await _db.CaLamViec.AnyAsync(x => x.IdChuyenKhoa == request.IdChuyenKhoa, cancellationToken);

        if (dangDuocSuDung)
        {
            throw new ConflictException("Khong the xoa chuyen khoa dang duoc su dung.");
        }

        _db.ChuyenKhoa.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
