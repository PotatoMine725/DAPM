using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.BacSi.Queries.LayHoSoCuaToi;
using ClinicBooking.Application.UnitTests.Common;
using BacSiEntity = ClinicBooking.Domain.Entities.BacSi;
using ClinicBooking.Domain.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace ClinicBooking.Application.UnitTests.Features.BacSi.Queries.LayHoSoCuaToi;

public sealed class LayHoSoCuaToiHandlerTests
{
    [Fact]
    public async Task Handle_HopLe_TraVeHoSo()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var tk = TestDataSeeder.SeedTaiKhoan(db, VaiTro.BacSi);
        var ck = TestDataSeeder.SeedChuyenKhoa(db, "CK-UT-BacSi-Profile-20260422");
        var bacSi = new BacSiEntity
        {
            IdTaiKhoan = tk.IdTaiKhoan,
            IdChuyenKhoa = ck.IdChuyenKhoa,
            HoTen = "Bac Si Profile UT",
            AnhDaiDien = "avatar.png",
            BangCap = "MD",
            NamKinhNghiem = 10,
            TieuSu = "Mo ta profile",
            LoaiHopDong = LoaiHopDong.NoiTru,
            TrangThai = TrangThaiBacSi.DangLam,
            NgayTao = DateTime.UtcNow
        };
        db.BacSi.Add(bacSi);
        await db.SaveChangesAsync();

        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.IdTaiKhoan.Returns(tk.IdTaiKhoan);

        var handler = new LayHoSoCuaToiHandler(db, currentUser);
        var result = await handler.Handle(new LayHoSoCuaToiQuery(), CancellationToken.None);

        result.HoTen.Should().Be("Bac Si Profile UT");
        result.TenChuyenKhoa.Should().Be("CK-UT-BacSi-Profile-20260422");
    }

    [Fact]
    public async Task Handle_KhongCoTaiKhoan_ThrowForbidden()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.IdTaiKhoan.Returns((int?)null);

        var handler = new LayHoSoCuaToiHandler(db, currentUser);

        var act = async () => await handler.Handle(new LayHoSoCuaToiQuery(), CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>();
    }
}
