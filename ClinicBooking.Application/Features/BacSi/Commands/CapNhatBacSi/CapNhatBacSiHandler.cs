using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Common.Exceptions;
using BacSiEntity = ClinicBooking.Domain.Entities.BacSi;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.BacSi.Commands.CapNhatBacSi;

public sealed class CapNhatBacSiHandler : IRequestHandler<CapNhatBacSiCommand>
{
    private readonly IAppDbContext _db;

    public CapNhatBacSiHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task Handle(CapNhatBacSiCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.BacSi.FirstOrDefaultAsync(x => x.IdBacSi == request.IdBacSi, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay bac si.");

        var chuyenKhoaTonTai = await _db.ChuyenKhoa.AnyAsync(x => x.IdChuyenKhoa == request.IdChuyenKhoa, cancellationToken);
        if (!chuyenKhoaTonTai)
        {
            throw new NotFoundException("Khong tim thay chuyen khoa.");
        }

        entity.IdChuyenKhoa = request.IdChuyenKhoa;
        entity.HoTen = request.HoTen;
        entity.AnhDaiDien = request.AnhDaiDien;
        entity.BangCap = request.BangCap;
        entity.NamKinhNghiem = request.NamKinhNghiem;
        entity.TieuSu = request.TieuSu;
        entity.LoaiHopDong = Enum.Parse<LoaiHopDong>(request.LoaiHopDong, true);
        entity.TrangThai = Enum.Parse<TrangThaiBacSi>(request.TrangThai, true);

        await _db.SaveChangesAsync(cancellationToken);
    }
}
