using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Constants;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.Auth.Dtos;
using ClinicBooking.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.Auth.Commands.KichHoatTaiKhoanWalkIn;

public class KichHoatTaiKhoanWalkInHandler : IRequestHandler<KichHoatTaiKhoanWalkInCommand, XacThucResponse>
{
    private readonly IAppDbContext _db;
    private readonly IOtpService _otpService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public KichHoatTaiKhoanWalkInHandler(
        IAppDbContext db,
        IOtpService otpService,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IDateTimeProvider dateTimeProvider)
    {
        _db = db;
        _otpService = otpService;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<XacThucResponse> Handle(KichHoatTaiKhoanWalkInCommand request, CancellationToken cancellationToken)
    {
        // 1. Load ghost account + BenhNhan
        var taiKhoan = await _db.TaiKhoan
            .Include(x => x.BenhNhan)
            .FirstOrDefaultAsync(x => x.IdTaiKhoan == request.IdTaiKhoan, cancellationToken);

        if (taiKhoan is null)
            throw new NotFoundException("TaiKhoan", request.IdTaiKhoan);

        // 2. Guard: must be a ghost account
        if (taiKhoan.TrangThai || !taiKhoan.TenDangNhap.StartsWith("walkin_"))
            throw new ForbiddenException("Tai khoan nay khong phai ghost account.");

        // 3. Verify OTP
        var otpHopLe = await _otpService.XacThucOtpDatLichAsync(request.IdTaiKhoan, request.MaOtp, cancellationToken);
        if (!otpHopLe)
            throw new UnauthorizedAccessException("Ma OTP khong hop le hoac da het han.");

        // 4. Verify identity
        var benhNhan = taiKhoan.BenhNhan;
        if (benhNhan is null)
            throw new NotFoundException("Ho so benh nhan tuong ung khong ton tai.");

        if (!XacMinhDanhTinh(request, benhNhan))
            throw new ForbiddenException("Thong tin xac minh danh tinh khong chinh xac.");

        // 5. Check new credentials uniqueness (exclude self)
        var tenDangNhapTrung = await _db.TaiKhoan
            .AnyAsync(x => x.TenDangNhap == request.TenDangNhap && x.IdTaiKhoan != request.IdTaiKhoan, cancellationToken);
        if (tenDangNhapTrung)
            throw new ConflictException("Ten dang nhap da ton tai.");

        var emailTrung = await _db.TaiKhoan
            .AnyAsync(x => x.Email == request.Email && !x.Email.EndsWith("@local.invalid") && x.IdTaiKhoan != request.IdTaiKhoan, cancellationToken);
        if (emailTrung)
            throw new ConflictException("Email da duoc su dung.");

        // 6. Activate in-place
        var now = _dateTimeProvider.UtcNow;
        taiKhoan.TenDangNhap = request.TenDangNhap;
        taiKhoan.Email = request.Email;
        taiKhoan.MatKhau = _passwordHasher.HashPassword(request.MatKhau);
        taiKhoan.TrangThai = true;
        taiKhoan.LanDangNhapCuoi = now;

        // 7. Create new RefreshToken
        var accessToken = _tokenService.TaoAccessToken(taiKhoan);
        var refreshToken = _tokenService.TaoRefreshToken();

        _db.RefreshToken.Add(new RefreshToken
        {
            IdTaiKhoan = taiKhoan.IdTaiKhoan,
            Token = refreshToken.Token,
            HetHan = refreshToken.HetHan,
            DaThuHoi = false,
            NgayTao = now
        });

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            // Concurrent registration hit unique constraint
            throw new ConflictException("Ten dang nhap hoac email da duoc su dung boi tai khoan khac.");
        }

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

    private static bool XacMinhDanhTinh(KichHoatTaiKhoanWalkInCommand request, ClinicBooking.Domain.Entities.BenhNhan benhNhan)
    {
        if (!string.IsNullOrWhiteSpace(request.Cccd) && !string.IsNullOrWhiteSpace(benhNhan.Cccd))
            return string.Equals(benhNhan.Cccd.Trim(), request.Cccd.Trim(), StringComparison.OrdinalIgnoreCase);

        if (request.NgaySinh.HasValue && !string.IsNullOrWhiteSpace(request.HoTen))
            return benhNhan.NgaySinh == request.NgaySinh &&
                   string.Equals(ChuanHoaTen(benhNhan.HoTen), ChuanHoaTen(request.HoTen), StringComparison.OrdinalIgnoreCase);

        return false;
    }

    private static string ChuanHoaTen(string ten) =>
        ten.Trim().ToUpperInvariant().Replace("  ", " ");
}
