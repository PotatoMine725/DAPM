using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.BenhNhan.Queries.LayHoSoCuaToi;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Enums;
using FluentAssertions;
using NSubstitute;

namespace ClinicBooking.Application.UnitTests.Features.BenhNhan.Queries.LayHoSoCuaToi;

public sealed class LayHoSoCuaToiHandlerTests
{
    [Fact]
    public async Task Handle_HopLe_TraVeHoSo()
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
            Cccd = "123456789012",
            NgaySinh = new DateOnly(1995, 1, 1),
            GioiTinh = GioiTinh.Nam,
            DiaChi = "123 Duong ABC",
            SoLanHuyMuon = 2,
            BiHanChe = true,
            NgayHetHanChe = new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc),
            NgayTao = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.IdTaiKhoan.Returns(taiKhoan.IdTaiKhoan);

        var handler = new LayHoSoCuaToiHandler(db, currentUser);
        var result = await handler.Handle(new LayHoSoCuaToiQuery(), CancellationToken.None);

        result.HoTen.Should().Be("Nguyen Van A");
        result.SoDienThoai.Should().Be("0912345678");
        result.Email.Should().Be("benhnhan_a@example.com");
        result.Cccd.Should().Be("123456789012");
        result.NgaySinh.Should().Be(new DateOnly(1995, 1, 1));
        result.GioiTinh.Should().Be(GioiTinh.Nam);
        result.DiaChi.Should().Be("123 Duong ABC");
        result.SoLanHuyMuon.Should().Be(2);
        result.BiHanChe.Should().BeTrue();
        result.NgayHetHanChe.Should().Be(new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc));
    }

    [Fact]
    public async Task Handle_KhongXacDinhNguoiDung_ThrowForbiddenException()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.IdTaiKhoan.Returns((int?)null);

        var handler = new LayHoSoCuaToiHandler(db, currentUser);
        var act = async () => await handler.Handle(new LayHoSoCuaToiQuery(), CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>()
            .WithMessage("Khong xac dinh duoc nguoi dung hien tai.");
    }
}
