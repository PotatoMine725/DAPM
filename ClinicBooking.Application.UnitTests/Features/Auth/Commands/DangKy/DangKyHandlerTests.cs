using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.Auth.Commands.DangKy;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Entities;
using ClinicBooking.Domain.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace ClinicBooking.Application.UnitTests.Features.Auth.Commands.DangKy;

public sealed class DangKyHandlerTests
{
    private static readonly DateTime FixedNow = new(2026, 5, 1, 10, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task Handle_HopLe_TaoTaiKhoanBenhNhanVaTraToken()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var passwordHasher = Substitute.For<IPasswordHasher>();
        passwordHasher.HashPassword("MatKhau#123").Returns("hashed_pw");

        var tokenService = Substitute.For<ITokenService>();
        tokenService.TaoAccessToken(Arg.Any<TaiKhoan>())
            .Returns(new AccessTokenResult("access_token", FixedNow.AddHours(1)));
        tokenService.TaoRefreshToken()
            .Returns(new RefreshTokenResult("refresh_token", FixedNow.AddDays(7)));

        var clock = Substitute.For<IDateTimeProvider>();
        clock.UtcNow.Returns(FixedNow);

        var handler = new DangKyHandler(db, passwordHasher, tokenService, clock);
        var result = await handler.Handle(
            new DangKyCommand(
                "benhnhan_a",
                "benhnhan_a@example.com",
                "0912345678",
                "MatKhau#123",
                "Nguyen Van A",
                new DateOnly(1995, 1, 1),
                GioiTinh.Nam,
                "123456789012",
                "123 Duong ABC"),
            CancellationToken.None);

        result.TenDangNhap.Should().Be("benhnhan_a");
        result.Email.Should().Be("benhnhan_a@example.com");
        result.VaiTro.Should().Be("benh_nhan");
        result.AccessToken.Should().Be("access_token");
        result.RefreshToken.Should().Be("refresh_token");

        var taiKhoan = await db.TaiKhoan.AsNoTracking()
            .SingleAsync(x => x.TenDangNhap == "benhnhan_a");
        taiKhoan.VaiTro.Should().Be(VaiTro.BenhNhan);
        taiKhoan.MatKhau.Should().Be("hashed_pw");
        taiKhoan.NgayTao.Should().Be(FixedNow);

        var benhNhan = await db.BenhNhan.AsNoTracking()
            .SingleAsync(x => x.IdTaiKhoan == taiKhoan.IdTaiKhoan);
        benhNhan.IdTaiKhoan.Should().Be(taiKhoan.IdTaiKhoan);
        benhNhan.HoTen.Should().Be("Nguyen Van A");
        benhNhan.SoLanHuyMuon.Should().Be(0);
        benhNhan.BiHanChe.Should().BeFalse();
        benhNhan.NgayTao.Should().Be(FixedNow);

        var refreshToken = await db.RefreshToken.AsNoTracking()
            .SingleAsync(x => x.IdTaiKhoan == taiKhoan.IdTaiKhoan);
        refreshToken.IdTaiKhoan.Should().Be(taiKhoan.IdTaiKhoan);
        refreshToken.Token.Should().Be("refresh_token");
        refreshToken.DaThuHoi.Should().BeFalse();
        refreshToken.NgayTao.Should().Be(FixedNow);

        passwordHasher.Received(1).HashPassword("MatKhau#123");
    }

    [Fact]
    public async Task Handle_EmailTrung_ThrowConflict()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        TestDataSeeder.SeedTaiKhoan(
            db,
            VaiTro.BenhNhan,
            tenDangNhap: "existing_user",
            email: "trung@example.com",
            soDienThoai: "0911111111");

        var handler = new DangKyHandler(
            db,
            Substitute.For<IPasswordHasher>(),
            Substitute.For<ITokenService>(),
            Substitute.For<IDateTimeProvider>());

        var act = async () => await handler.Handle(
            new DangKyCommand(
                "new_user",
                "trung@example.com",
                "0922222222",
                "MatKhau#123",
                "New User",
                null,
                null,
                null,
                null),
            CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("Email da duoc su dung.");
    }

    [Fact]
    public async Task Handle_CccdTrung_ThrowConflict()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var tk = TestDataSeeder.SeedTaiKhoan(db, VaiTro.BenhNhan, tenDangNhap: "u1", email: "u1@example.com", soDienThoai: "0933333333");
        db.BenhNhan.Add(new ClinicBooking.Domain.Entities.BenhNhan
        {
            IdTaiKhoan = tk.IdTaiKhoan,
            HoTen = "Existing",
            Cccd = "123456789012",
            NgayTao = FixedNow
        });
        await db.SaveChangesAsync();

        var handler = new DangKyHandler(
            db,
            Substitute.For<IPasswordHasher>(),
            Substitute.For<ITokenService>(),
            Substitute.For<IDateTimeProvider>());

        var act = async () => await handler.Handle(
            new DangKyCommand(
                "new_user",
                "new_user@example.com",
                "0944444444",
                "MatKhau#123",
                "New User",
                null,
                null,
                "123456789012",
                null),
            CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("CCCD da duoc su dung.");
    }
}
