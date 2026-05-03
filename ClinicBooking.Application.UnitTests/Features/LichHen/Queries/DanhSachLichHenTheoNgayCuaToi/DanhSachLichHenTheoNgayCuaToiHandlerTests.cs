using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.LichHen.Queries.DanhSachLichHenTheoNgayCuaToi;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Enums;
using FluentAssertions;
using NSubstitute;

namespace ClinicBooking.Application.UnitTests.Features.LichHen.Queries.DanhSachLichHenTheoNgayCuaToi;

public sealed class DanhSachLichHenTheoNgayCuaToiHandlerTests
{
    [Fact]
    public async Task Handle_BacSi_ChiTraVeLichHenCuaChinhMinh()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var bacSi1 = TestDataSeeder.SeedBacSi(db);
        var bacSi2 = TestDataSeeder.SeedBacSi(db);
        var ca1 = TestDataSeeder.SeedCaLamViec(db, new DateOnly(2026, 5, 5));
        var ca2 = TestDataSeeder.SeedCaLamViec(db, new DateOnly(2026, 5, 5));
        ca1.IdBacSi = bacSi1.IdBacSi;
        ca2.IdBacSi = bacSi2.IdBacSi;
        db.SaveChanges();

        var bn = TestDataSeeder.SeedBenhNhan(db);
        TestDataSeeder.SeedLichHen(db, bn.IdBenhNhan, ca1.IdCaLamViec, soSlot: 1);
        TestDataSeeder.SeedLichHen(db, bn.IdBenhNhan, ca2.IdCaLamViec, soSlot: 2);

        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.IdTaiKhoan.Returns(bacSi1.IdTaiKhoan);
        currentUser.VaiTro.Returns(VaiTro.BacSi);

        var handler = new DanhSachLichHenTheoNgayCuaToiHandler(db, currentUser);

        var result = await handler.Handle(new DanhSachLichHenTheoNgayCuaToiQuery(new DateOnly(2026, 5, 5)), CancellationToken.None);

        result.Should().HaveCount(1);
        result.Should().OnlyContain(x => x.IdCaLamViec == ca1.IdCaLamViec);
    }

    [Fact]
    public async Task Handle_Admin_TraVeTatCaLichHenTrongNgay()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var ca1 = TestDataSeeder.SeedCaLamViec(db, new DateOnly(2026, 5, 5));
        var ca2 = TestDataSeeder.SeedCaLamViec(db, new DateOnly(2026, 5, 5));
        var bn = TestDataSeeder.SeedBenhNhan(db);
        TestDataSeeder.SeedLichHen(db, bn.IdBenhNhan, ca1.IdCaLamViec, soSlot: 1);
        TestDataSeeder.SeedLichHen(db, bn.IdBenhNhan, ca2.IdCaLamViec, soSlot: 2);

        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.IdTaiKhoan.Returns((int?)null);
        currentUser.VaiTro.Returns(VaiTro.Admin);

        var handler = new DanhSachLichHenTheoNgayCuaToiHandler(db, currentUser);

        var result = await handler.Handle(new DanhSachLichHenTheoNgayCuaToiQuery(new DateOnly(2026, 5, 5)), CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_KhachKhongCoQuyen_ThrowsForbidden()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.IdTaiKhoan.Returns((int?)null);
        currentUser.VaiTro.Returns(VaiTro.BenhNhan);

        var handler = new DanhSachLichHenTheoNgayCuaToiHandler(db, currentUser);

        var act = () => handler.Handle(new DanhSachLichHenTheoNgayCuaToiQuery(new DateOnly(2026, 5, 5)), CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>();
    }
}
