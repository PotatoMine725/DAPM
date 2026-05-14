using System.Security.Claims;
using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Constants;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.Auth.Dtos;
using ClinicBooking.Domain.Entities;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.Auth.Commands.LamMoiToken;

public class LamMoiTokenHandler : IRequestHandler<LamMoiTokenCommand, XacThucResponse>
{
    private readonly IAppDbContext _db;
    private readonly ITokenService _tokenService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public LamMoiTokenHandler(
        IAppDbContext db,
        ITokenService tokenService,
        IDateTimeProvider dateTimeProvider)
    {
        _db = db;
        _tokenService = tokenService;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<XacThucResponse> Handle(
        LamMoiTokenCommand request,
        CancellationToken cancellationToken)
    {
        var tokenHienTai = await _db.RefreshToken
            .Include(x => x.TaiKhoan)
            .FirstOrDefaultAsync(x => x.Token == request.RefreshToken, cancellationToken);

        if (tokenHienTai is null)
        {
            throw new UnauthorizedAccessException("Refresh token khong ton tai.");
        }

        var now = _dateTimeProvider.UtcNow;

        if (tokenHienTai.DaThuHoi)
        {
            // Phat hien tai su dung: thu hoi toan bo token cua tai khoan.
            await _db.RefreshToken
                .Where(x => x.IdTaiKhoan == tokenHienTai.IdTaiKhoan && !x.DaThuHoi)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(x => x.DaThuHoi, true)
                    .SetProperty(x => x.NgayThuHoi, now)
                    .SetProperty(x => x.LyDoThuHoi, "Phat hien tai su dung refresh token da thu hoi."),
                    cancellationToken);

            throw new UnauthorizedAccessException("Refresh token khong hop le.");
        }

        if (tokenHienTai.HetHan <= now)
        {
            throw new UnauthorizedAccessException("Refresh token da het han.");
        }

        var taiKhoan = tokenHienTai.TaiKhoan;
        if (!taiKhoan.TrangThai)
        {
            throw new ForbiddenException("Tai khoan da bi khoa.");
        }

        var claimsThem = new List<Claim>();
        if (taiKhoan.VaiTro == VaiTro.BacSi)
        {
            var bacSi = await _db.BacSi
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.IdTaiKhoan == taiKhoan.IdTaiKhoan, cancellationToken);
            if (bacSi is not null)
                claimsThem.Add(new Claim("loai_hop_dong", bacSi.LoaiHopDong.ToString()));
        }

        var accessToken = _tokenService.TaoAccessToken(taiKhoan, claimsThem.Count > 0 ? claimsThem : null);
        var refreshTokenMoi = _tokenService.TaoRefreshToken();

        tokenHienTai.DaThuHoi = true;
        tokenHienTai.NgayThuHoi = now;
        tokenHienTai.LyDoThuHoi = "Thay the boi token moi.";
        tokenHienTai.ThayTheBangToken = refreshTokenMoi.Token;

        _db.RefreshToken.Add(new RefreshToken
        {
            IdTaiKhoan = taiKhoan.IdTaiKhoan,
            Token = refreshTokenMoi.Token,
            HetHan = refreshTokenMoi.HetHan,
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
            refreshTokenMoi.Token,
            refreshTokenMoi.HetHan);
    }
}
