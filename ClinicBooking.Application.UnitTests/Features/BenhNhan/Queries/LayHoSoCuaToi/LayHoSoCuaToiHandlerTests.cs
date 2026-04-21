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
            NgayTao = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.IdTaiKhoan.Returns(taiKhoan.IdTaiKhoan);

        var handler = new LayHoSoCuaToiHandler(db, currentUser);
        var result = await handler.Handle(new LayHoSoCuaToiQuery(), CancellationToken.None);

        result.HoTen.Should().Be("Nguyen Van A");
        result.SoDienThoai.Should().Be("0912345678");
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
