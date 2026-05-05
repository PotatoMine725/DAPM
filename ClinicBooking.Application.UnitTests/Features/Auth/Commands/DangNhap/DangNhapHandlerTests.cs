using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.Auth.Commands.DangNhap;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Entities;
using ClinicBooking.Domain.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace ClinicBooking.Application.UnitTests.Features.Auth.Commands.DangNhap;

public sealed class DangNhapHandlerTests
{
    private static readonly DateTime FixedNow = new(2026, 5, 1, 10, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task Handle_HopLe_TraTokenVaCapNhatLanDangNhapCuoi()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var taiKhoan = new TaiKhoan
        {
            TenDangNhap = "benhnhan_a",
            Email = "benhnhan_a@example.com",
            SoDienThoai = "0912345678",
            MatKhau = "hashed_pw",
            VaiTro = VaiTro.BenhNhan,
            TrangThai = true,
            NgayTao = FixedNow.AddDays(-1)
        };
        db.TaiKhoan.Add(taiKhoan);
        await db.SaveChangesAsync();

        var passwordHasher = Substitute.For<IPasswordHasher>();
        passwordHasher.VerifyPassword("MatKhau#123", "hashed_pw").Returns(true);

        var tokenService = Substitute.For<ITokenService>();
        tokenService.TaoAccessToken(Arg.Any<TaiKhoan>())
            .Returns(new AccessTokenResult("access_token", FixedNow.AddHours(1)));
        tokenService.TaoRefreshToken()
            .Returns(new RefreshTokenResult("refresh_token", FixedNow.AddDays(7)));

        var clock = Substitute.For<IDateTimeProvider>();
        clock.UtcNow.Returns(FixedNow);

        var logger = Substitute.For<ILogger<DangNhapHandler>>();

        var handler = new DangNhapHandler(db, passwordHasher, tokenService, clock, logger);
        var result = await handler.Handle(
            new DangNhapCommand("  benhnhan_a  ", "MatKhau#123"),
            CancellationToken.None);

        result.IdTaiKhoan.Should().Be(taiKhoan.IdTaiKhoan);
        result.VaiTro.Should().Be("benh_nhan");
        result.AccessToken.Should().Be("access_token");
        result.RefreshToken.Should().Be("refresh_token");

        var taiKhoanSau = await db.TaiKhoan.AsNoTracking()
            .SingleAsync(x => x.IdTaiKhoan == taiKhoan.IdTaiKhoan);
        taiKhoanSau.LanDangNhapCuoi.Should().Be(FixedNow);

        var refreshToken = await db.RefreshToken.AsNoTracking()
            .SingleAsync(x => x.IdTaiKhoan == taiKhoan.IdTaiKhoan);
        refreshToken.IdTaiKhoan.Should().Be(taiKhoan.IdTaiKhoan);
        refreshToken.Token.Should().Be("refresh_token");
        refreshToken.DaThuHoi.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_TaiKhoanKhongTonTai_ThrowUnauthorized()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var handler = new DangNhapHandler(
            db,
            Substitute.For<IPasswordHasher>(),
            Substitute.For<ITokenService>(),
            Substitute.For<IDateTimeProvider>(),
            Substitute.For<ILogger<DangNhapHandler>>());

        var act = async () => await handler.Handle(
            new DangNhapCommand("khong_ton_tai", "abc"),
            CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Tai khoan hoac mat khau khong dung.");
    }

    [Fact]
    public async Task Handle_TaiKhoanBiKhoa_ThrowForbidden()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        db.TaiKhoan.Add(new TaiKhoan
        {
            TenDangNhap = "blocked_user",
            Email = "blocked@example.com",
            SoDienThoai = "0911111111",
            MatKhau = "hash",
            VaiTro = VaiTro.BenhNhan,
            TrangThai = false,
            NgayTao = FixedNow
        });
        await db.SaveChangesAsync();

        var handler = new DangNhapHandler(
            db,
            Substitute.For<IPasswordHasher>(),
            Substitute.For<ITokenService>(),
            Substitute.For<IDateTimeProvider>(),
            Substitute.For<ILogger<DangNhapHandler>>());

        var act = async () => await handler.Handle(
            new DangNhapCommand("blocked_user", "abc"),
            CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>()
            .WithMessage("Tai khoan da bi khoa.");
    }

    [Fact]
    public async Task Handle_SaiMatKhau_ThrowUnauthorized()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        db.TaiKhoan.Add(new TaiKhoan
        {
            TenDangNhap = "user_a",
            Email = "user_a@example.com",
            SoDienThoai = "0922222222",
            MatKhau = "hashed_pw",
            VaiTro = VaiTro.BenhNhan,
            TrangThai = true,
            NgayTao = FixedNow
        });
        await db.SaveChangesAsync();

        var passwordHasher = Substitute.For<IPasswordHasher>();
        passwordHasher.VerifyPassword("wrong_pw", "hashed_pw").Returns(false);

        var handler = new DangNhapHandler(
            db,
            passwordHasher,
            Substitute.For<ITokenService>(),
            Substitute.For<IDateTimeProvider>(),
            Substitute.For<ILogger<DangNhapHandler>>());

        var act = async () => await handler.Handle(
            new DangNhapCommand("user_a", "wrong_pw"),
            CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Tai khoan hoac mat khau khong dung.");
    }
}
