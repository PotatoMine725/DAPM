using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.BenhNhan.Commands.CapNhatHoSoCuaToi;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace ClinicBooking.Application.UnitTests.Features.BenhNhan.Commands.CapNhatHoSoCuaToi;

public sealed class CapNhatHoSoCuaToiHandlerTests
{
    [Fact]
    public async Task Handle_HopLe_CapNhatThanhCong()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var taiKhoan = new ClinicBooking.Domain.Entities.TaiKhoan
        {
            TenDangNhap = "benhnhan_a",
            Email = "benhnhan_a@example.com",
            SoDienThoai = "0912345678",
            MatKhau = "hash",
            VaiTro = VaiTro.BenhNhan,
            TrangThai = true,
            NgayTao = DateTime.UtcNow
        };
        db.TaiKhoan.Add(taiKhoan);
        await db.SaveChangesAsync();

        db.BenhNhan.Add(new ClinicBooking.Domain.Entities.BenhNhan
        {
            IdTaiKhoan = taiKhoan.IdTaiKhoan,
            HoTen = "Nguyen Van A",
            NgayTao = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.IdTaiKhoan.Returns(taiKhoan.IdTaiKhoan);

        var handler = new CapNhatHoSoCuaToiHandler(db, currentUser);

        await handler.Handle(
            new CapNhatHoSoCuaToiCommand(
                "Nguyen Van B",
                new DateOnly(1995, 1, 1),
                GioiTinh.Nam,
                "123456789012",
                "Dia chi moi"),
            CancellationToken.None);

        var entity = await db.BenhNhan.AsNoTracking().FirstAsync(x => x.IdTaiKhoan == taiKhoan.IdTaiKhoan);
        entity.HoTen.Should().Be("Nguyen Van B");
        entity.Cccd.Should().Be("123456789012");
    }

    [Fact]
    public async Task Handle_CccdTrung_ThrowConflictException()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var taiKhoan1 = new ClinicBooking.Domain.Entities.TaiKhoan
        {
            TenDangNhap = "benhnhan_1",
            Email = "benhnhan_1@example.com",
            SoDienThoai = "0912345678",
            MatKhau = "hash",
            VaiTro = VaiTro.BenhNhan,
            TrangThai = true,
            NgayTao = DateTime.UtcNow
        };
        var taiKhoan2 = new ClinicBooking.Domain.Entities.TaiKhoan
        {
            TenDangNhap = "benhnhan_2",
            Email = "benhnhan_2@example.com",
            SoDienThoai = "0912345679",
            MatKhau = "hash",
            VaiTro = VaiTro.BenhNhan,
            TrangThai = true,
            NgayTao = DateTime.UtcNow
        };
        db.TaiKhoan.AddRange(taiKhoan1, taiKhoan2);
        await db.SaveChangesAsync();

        db.BenhNhan.Add(new ClinicBooking.Domain.Entities.BenhNhan
        {
            IdTaiKhoan = taiKhoan1.IdTaiKhoan,
            HoTen = "A",
            Cccd = "123456789012",
            NgayTao = DateTime.UtcNow
        });
        db.BenhNhan.Add(new ClinicBooking.Domain.Entities.BenhNhan
        {
            IdTaiKhoan = taiKhoan2.IdTaiKhoan,
            HoTen = "B",
            NgayTao = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.IdTaiKhoan.Returns(taiKhoan2.IdTaiKhoan);

        var handler = new CapNhatHoSoCuaToiHandler(db, currentUser);
        var act = async () => await handler.Handle(
            new CapNhatHoSoCuaToiCommand("B", null, null, "123456789012", null),
            CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("CCCD da duoc su dung.");
    }
}
