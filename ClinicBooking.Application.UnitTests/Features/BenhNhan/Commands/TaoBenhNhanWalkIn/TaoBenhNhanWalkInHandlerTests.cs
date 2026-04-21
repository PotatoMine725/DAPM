using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.BenhNhan.Commands.TaoBenhNhanWalkIn;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace ClinicBooking.Application.UnitTests.Features.BenhNhan.Commands.TaoBenhNhanWalkIn;

public sealed class TaoBenhNhanWalkInHandlerTests
{
    [Fact]
    public async Task Handle_HopLe_TaoThanhCong()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var passwordHasher = Substitute.For<IPasswordHasher>();
        passwordHasher.HashPassword(Arg.Any<string>()).Returns("hashed_password");
        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider.UtcNow.Returns(new DateTime(2026, 4, 21, 7, 0, 0, DateTimeKind.Utc));

        var handler = new TaoBenhNhanWalkInHandler(db, passwordHasher, dateTimeProvider);

        var result = await handler.Handle(
            new TaoBenhNhanWalkInCommand(
                "Nguyen Van A",
                "0912345678",
                new DateOnly(1995, 1, 1),
                GioiTinh.Nam,
                "123456789012",
                "Dia chi"),
            CancellationToken.None);

        result.IdBenhNhan.Should().BeGreaterThan(0);
        result.IdTaiKhoan.Should().BeGreaterThan(0);

        var taiKhoan = await db.TaiKhoan.AsNoTracking().FirstAsync(x => x.IdTaiKhoan == result.IdTaiKhoan);
        taiKhoan.VaiTro.Should().Be(VaiTro.BenhNhan);
        taiKhoan.TrangThai.Should().BeFalse();
        taiKhoan.SoDienThoai.Should().Be("0912345678");
    }

    [Fact]
    public async Task Handle_TrungSoDienThoai_ThrowConflictException()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        db.TaiKhoan.Add(new ClinicBooking.Domain.Entities.TaiKhoan
        {
            TenDangNhap = "ton_tai",
            Email = "ton_tai@example.com",
            SoDienThoai = "0912345678",
            MatKhau = "hash",
            VaiTro = VaiTro.BenhNhan,
            TrangThai = true,
            NgayTao = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        var passwordHasher = Substitute.For<IPasswordHasher>();
        passwordHasher.HashPassword(Arg.Any<string>()).Returns("hashed_password");
        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);

        var handler = new TaoBenhNhanWalkInHandler(db, passwordHasher, dateTimeProvider);
        var act = async () => await handler.Handle(
            new TaoBenhNhanWalkInCommand("A", "0912345678", null, null, null, null),
            CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("So dien thoai da duoc su dung.");
    }
}
