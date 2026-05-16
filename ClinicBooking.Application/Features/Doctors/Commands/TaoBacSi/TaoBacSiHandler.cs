using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Domain.Entities;
using BacSiEntity = ClinicBooking.Domain.Entities.BacSi;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.Doctors.Commands.TaoBacSi;

public sealed class TaoBacSiHandler : IRequestHandler<TaoBacSiCommand, int>
{
    private readonly IAppDbContext _db;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IDateTimeProvider _dateTimeProvider;

    public TaoBacSiHandler(
        IAppDbContext db,
        IPasswordHasher passwordHasher,
        IDateTimeProvider dateTimeProvider)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<int> Handle(TaoBacSiCommand request, CancellationToken cancellationToken)
    {
        var tenDangNhapTrung = await _db.TaiKhoan
            .AnyAsync(x => x.TenDangNhap == request.TenDangNhap, cancellationToken);
        if (tenDangNhapTrung)
        {
            throw new ConflictException("Ten dang nhap da ton tai.");
        }

        var emailTrung = await _db.TaiKhoan
            .AnyAsync(x => x.Email == request.Email, cancellationToken);
        if (emailTrung)
        {
            throw new ConflictException("Email da duoc su dung.");
        }

        var sdtTrung = await _db.TaiKhoan
            .AnyAsync(x => x.SoDienThoai == request.SoDienThoai, cancellationToken);
        if (sdtTrung)
        {
            throw new ConflictException("So dien thoai da duoc su dung.");
        }

        var chuyenKhoaTonTai = await _db.ChuyenKhoa
            .AnyAsync(x => x.IdChuyenKhoa == request.IdChuyenKhoa, cancellationToken);
        if (!chuyenKhoaTonTai)
        {
            throw new NotFoundException("Khong tim thay chuyen khoa.");
        }

        var now = _dateTimeProvider.UtcNow;

        var taiKhoan = new TaiKhoan
        {
            TenDangNhap = request.TenDangNhap.Trim(),
            Email = request.Email.Trim(),
            SoDienThoai = request.SoDienThoai.Trim(),
            MatKhau = _passwordHasher.HashPassword(request.MatKhau),
            VaiTro = VaiTro.BacSi,
            TrangThai = true,
            NgayTao = now
        };

        var bacSi = new BacSiEntity
        {
            TaiKhoan = taiKhoan,
            IdChuyenKhoa = request.IdChuyenKhoa,
            HoTen = request.HoTen.Trim(),
            BangCap = request.BangCap?.Trim(),
            NamKinhNghiem = request.NamKinhNghiem,
            TieuSu = request.TieuSu?.Trim(),
            LoaiHopDong = request.LoaiHopDong,
            TrangThai = TrangThaiBacSi.DangLam,
            NgayTao = now
        };

        _db.TaiKhoan.Add(taiKhoan);
        _db.BacSi.Add(bacSi);
        await _db.SaveChangesAsync(cancellationToken);

        return bacSi.IdBacSi;
    }
}
