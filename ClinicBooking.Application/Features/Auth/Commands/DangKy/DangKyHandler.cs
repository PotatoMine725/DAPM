using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Constants;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.Auth.Dtos;
using ClinicBooking.Domain.Entities;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.Auth.Commands.DangKy;

public class DangKyHandler : IRequestHandler<DangKyCommand, XacThucResponse>
{
    private readonly IAppDbContext _db;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public DangKyHandler(
        IAppDbContext db,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IDateTimeProvider dateTimeProvider)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<XacThucResponse> Handle(
        DangKyCommand request,
        CancellationToken cancellationToken)
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

        var soDienThoaiTrung = await _db.TaiKhoan
            .AnyAsync(x => x.SoDienThoai == request.SoDienThoai, cancellationToken);
        if (soDienThoaiTrung)
        {
            throw new ConflictException("So dien thoai da duoc su dung.");
        }

        if (!string.IsNullOrWhiteSpace(request.Cccd))
        {
            var cccdTrung = await _db.BenhNhan
                .AnyAsync(x => x.Cccd == request.Cccd, cancellationToken);
            if (cccdTrung)
            {
                throw new ConflictException("CCCD da duoc su dung.");
            }
        }

        var now = _dateTimeProvider.UtcNow;

        var taiKhoan = new TaiKhoan
        {
            TenDangNhap = request.TenDangNhap,
            Email = request.Email,
            SoDienThoai = request.SoDienThoai,
            MatKhau = _passwordHasher.HashPassword(request.MatKhau),
            VaiTro = VaiTro.BenhNhan,
            TrangThai = true,
            NgayTao = now
        };

        var benhNhan = new BenhNhan
        {
            TaiKhoan = taiKhoan,
            HoTen = request.HoTen,
            NgaySinh = request.NgaySinh,
            GioiTinh = request.GioiTinh,
            Cccd = request.Cccd,
            DiaChi = request.DiaChi,
            SoLanHuyMuon = 0,
            BiHanChe = false,
            NgayTao = now
        };

        _db.TaiKhoan.Add(taiKhoan);
        _db.BenhNhan.Add(benhNhan);

        var accessToken = _tokenService.TaoAccessToken(taiKhoan);
        var refreshToken = _tokenService.TaoRefreshToken();

        _db.RefreshToken.Add(new RefreshToken
        {
            TaiKhoan = taiKhoan,
            Token = refreshToken.Token,
            HetHan = refreshToken.HetHan,
            DaThuHoi = false,
            NgayTao = now
        });

        await _db.SaveChangesAsync(cancellationToken);

        return new XacThucResponse(
            taiKhoan.IdTaiKhoan,
            taiKhoan.TenDangNhap,
            taiKhoan.Email,
            taiKhoan.VaiTro.ToRoleClaim(),
            accessToken.Token,
            accessToken.HetHan,
            refreshToken.Token,
            refreshToken.HetHan);
    }
}
