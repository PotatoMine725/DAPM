using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.DanhMuc.Commands.TaoDichVu;

public sealed class TaoDichVuHandler : IRequestHandler<TaoDichVuCommand, int>
{
    private readonly IAppDbContext _db;

    public TaoDichVuHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<int> Handle(TaoDichVuCommand request, CancellationToken cancellationToken)
    {
        var chuyenKhoaTonTai = await _db.ChuyenKhoa
            .AnyAsync(x => x.IdChuyenKhoa == request.IdChuyenKhoa, cancellationToken);
        if (!chuyenKhoaTonTai)
        {
            throw new NotFoundException("Khong tim thay chuyen khoa.");
        }

        var tenDaTonTai = await _db.DichVu
            .AnyAsync(x => x.IdChuyenKhoa == request.IdChuyenKhoa && x.TenDichVu == request.TenDichVu, cancellationToken);
        if (tenDaTonTai)
        {
            throw new ConflictException("Ten dich vu da ton tai trong chuyen khoa nay.");
        }

        var entity = new DichVu
        {
            IdChuyenKhoa = request.IdChuyenKhoa,
            TenDichVu = request.TenDichVu,
            MoTa = request.MoTa,
            ThoiGianUocTinh = request.ThoiGianUocTinh,
            HienThi = request.HienThi,
            NgayTao = DateTime.UtcNow
        };

        _db.DichVu.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return entity.IdDichVu;
    }
}
