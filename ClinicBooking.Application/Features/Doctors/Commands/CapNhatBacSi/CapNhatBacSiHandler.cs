using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.Doctors.Commands.CapNhatBacSi;

public sealed class CapNhatBacSiHandler : IRequestHandler<CapNhatBacSiCommand>
{
    private readonly IAppDbContext _db;

    public CapNhatBacSiHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task Handle(CapNhatBacSiCommand request, CancellationToken cancellationToken)
    {
        var bacSi = await _db.BacSi
            .Include(x => x.TaiKhoan)
            .FirstOrDefaultAsync(x => x.IdBacSi == request.IdBacSi, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay bac si.");

        var chuyenKhoaTonTai = await _db.ChuyenKhoa
            .AnyAsync(x => x.IdChuyenKhoa == request.IdChuyenKhoa, cancellationToken);
        if (!chuyenKhoaTonTai)
        {
            throw new NotFoundException("Khong tim thay chuyen khoa.");
        }

        bacSi.HoTen = request.HoTen.Trim();
        bacSi.IdChuyenKhoa = request.IdChuyenKhoa;
        bacSi.LoaiHopDong = request.LoaiHopDong;
        bacSi.TrangThai = request.TrangThai;
        bacSi.BangCap = request.BangCap?.Trim();
        bacSi.NamKinhNghiem = request.NamKinhNghiem;
        bacSi.TieuSu = request.TieuSu?.Trim();

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var emailMoi = request.Email.Trim();
            if (emailMoi != bacSi.TaiKhoan.Email)
            {
                var emailTrung = await _db.TaiKhoan
                    .AnyAsync(x => x.Email == emailMoi && x.IdTaiKhoan != bacSi.IdTaiKhoan, cancellationToken);
                if (emailTrung)
                {
                    throw new ConflictException("Email da duoc su dung.");
                }
                bacSi.TaiKhoan.Email = emailMoi;
            }
        }

        if (!string.IsNullOrWhiteSpace(request.SoDienThoai))
        {
            var sdtMoi = request.SoDienThoai.Trim();
            if (sdtMoi != bacSi.TaiKhoan.SoDienThoai)
            {
                var sdtTrung = await _db.TaiKhoan
                    .AnyAsync(x => x.SoDienThoai == sdtMoi && x.IdTaiKhoan != bacSi.IdTaiKhoan, cancellationToken);
                if (sdtTrung)
                {
                    throw new ConflictException("So dien thoai da duoc su dung.");
                }
                bacSi.TaiKhoan.SoDienThoai = sdtMoi;
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
    }
}
