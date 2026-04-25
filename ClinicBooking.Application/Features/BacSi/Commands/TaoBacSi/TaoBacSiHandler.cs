using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Domain.Enums;
using BacSiEntity = ClinicBooking.Domain.Entities.BacSi;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.BacSi.Commands.TaoBacSi;

public sealed class TaoBacSiHandler : IRequestHandler<TaoBacSiCommand, int>
{
    private readonly IAppDbContext _db;

    public TaoBacSiHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<int> Handle(TaoBacSiCommand request, CancellationToken cancellationToken)
    {
        var taiKhoan = await _db.TaiKhoan.FirstOrDefaultAsync(x => x.IdTaiKhoan == request.IdTaiKhoan, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay tai khoan.");

        if (taiKhoan.VaiTro != VaiTro.BacSi)
        {
            throw new ConflictException("Tai khoan nay khong co vai tro bac si.");
        }

        var chuyenKhoaTonTai = await _db.ChuyenKhoa.AnyAsync(x => x.IdChuyenKhoa == request.IdChuyenKhoa, cancellationToken);
        if (!chuyenKhoaTonTai)
        {
            throw new NotFoundException("Khong tim thay chuyen khoa.");
        }

        var entity = new BacSiEntity
        {
            IdTaiKhoan = request.IdTaiKhoan,
            IdChuyenKhoa = request.IdChuyenKhoa,
            HoTen = request.HoTen,
            AnhDaiDien = request.AnhDaiDien,
            BangCap = request.BangCap,
            NamKinhNghiem = request.NamKinhNghiem,
            TieuSu = request.TieuSu,
            LoaiHopDong = Enum.Parse<LoaiHopDong>(request.LoaiHopDong, true),
            TrangThai = Enum.Parse<TrangThaiBacSi>(request.TrangThai, true),
            NgayTao = DateTime.UtcNow
        };

        _db.BacSi.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return entity.IdBacSi;
    }
}
