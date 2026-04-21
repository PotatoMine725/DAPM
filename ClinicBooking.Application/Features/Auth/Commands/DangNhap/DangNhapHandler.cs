using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Constants;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.Auth.Dtos;
using ClinicBooking.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace ClinicBooking.Application.Features.Auth.Commands.DangNhap;

public class DangNhapHandler : IRequestHandler<DangNhapCommand, XacThucResponse>
{
    private readonly IAppDbContext _db;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<DangNhapHandler> _logger;

    public DangNhapHandler(
        IAppDbContext db,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IDateTimeProvider dateTimeProvider,
        ILogger<DangNhapHandler> logger)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    public async Task<XacThucResponse> Handle(
        DangNhapCommand request,
        CancellationToken cancellationToken)
    {
        var dinhDanh = request.TenDangNhapHoacEmail.Trim();

        var taiKhoan = await _db.TaiKhoan
            .FirstOrDefaultAsync(
                x => x.TenDangNhap == dinhDanh || x.Email == dinhDanh,
                cancellationToken);

        if (taiKhoan is null)
        {
            throw new UnauthorizedAccessException("Tai khoan hoac mat khau khong dung.");
        }

        if (!taiKhoan.TrangThai)
        {
            throw new ForbiddenException("Tai khoan da bi khoa.");
        }

        if (!_passwordHasher.VerifyPassword(request.MatKhau, taiKhoan.MatKhau))
        {
            throw new UnauthorizedAccessException("Tai khoan hoac mat khau khong dung.");
        }

        var now = _dateTimeProvider.UtcNow;
        taiKhoan.LanDangNhapCuoi = now;

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
