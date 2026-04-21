using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.LichHen.Queries.DanhSachLichHenCuaToi;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Enums;
using FluentAssertions;
using NSubstitute;

namespace ClinicBooking.Application.UnitTests.Features.LichHen.Queries.DanhSachLichHenCuaToi;

public sealed class DanhSachLichHenCuaToiHandlerTests
{
    private static ICurrentUserService CurrentUser(int? idTaiKhoan)
    {
        var cu = Substitute.For<ICurrentUserService>();
        cu.IdTaiKhoan.Returns(idTaiKhoan);
        cu.VaiTro.Returns(VaiTro.BenhNhan);
        return cu;
    }

    [Fact]
    public async Task Handle_ChiTraVeLichHenCuaChinhMinh()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var tk = TestDataSeeder.SeedTaiKhoan(db, VaiTro.BenhNhan);
        var bn = TestDataSeeder.SeedBenhNhan(db, tk.IdTaiKhoan);
        var bnKhac = TestDataSeeder.SeedBenhNhan(db);
        var ca = TestDataSeeder.SeedCaLamViec(db);
        TestDataSeeder.SeedLichHen(db, bn.IdBenhNhan, ca.IdCaLamViec, soSlot: 1);
        TestDataSeeder.SeedLichHen(db, bn.IdBenhNhan, ca.IdCaLamViec, soSlot: 2);
        TestDataSeeder.SeedLichHen(db, bnKhac.IdBenhNhan, ca.IdCaLamViec, soSlot: 3);

        var handler = new DanhSachLichHenCuaToiHandler(db, CurrentUser(tk.IdTaiKhoan));
        var result = await handler.Handle(
            new DanhSachLichHenCuaToiQuery(null, 1, 20), CancellationToken.None);

        result.TongSo.Should().Be(2);
        result.KetQua.Should().OnlyContain(x => x.IdBenhNhan == bn.IdBenhNhan);
    }

    [Fact]
    public async Task Handle_LocTheoTrangThai()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var tk = TestDataSeeder.SeedTaiKhoan(db, VaiTro.BenhNhan);
        var bn = TestDataSeeder.SeedBenhNhan(db, tk.IdTaiKhoan);
        var ca = TestDataSeeder.SeedCaLamViec(db);
        TestDataSeeder.SeedLichHen(db, bn.IdBenhNhan, ca.IdCaLamViec, soSlot: 1, trangThai: TrangThaiLichHen.ChoXacNhan);
        TestDataSeeder.SeedLichHen(db, bn.IdBenhNhan, ca.IdCaLamViec, soSlot: 2, trangThai: TrangThaiLichHen.DaXacNhan);

        var handler = new DanhSachLichHenCuaToiHandler(db, CurrentUser(tk.IdTaiKhoan));
        var result = await handler.Handle(
            new DanhSachLichHenCuaToiQuery(TrangThaiLichHen.DaXacNhan, 1, 20), CancellationToken.None);

        result.TongSo.Should().Be(1);
        result.KetQua.Should().OnlyContain(x => x.TrangThai == TrangThaiLichHen.DaXacNhan);
    }

    [Fact]
    public async Task Handle_KhongXacThuc_ThrowForbiddenException()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var handler = new DanhSachLichHenCuaToiHandler(db, CurrentUser(null));

        var act = async () => await handler.Handle(
            new DanhSachLichHenCuaToiQuery(null, 1, 20), CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task Handle_TaiKhoanKhongCoBenhNhan_ThrowNotFoundException()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var tk = TestDataSeeder.SeedTaiKhoan(db, VaiTro.BenhNhan);
        var handler = new DanhSachLichHenCuaToiHandler(db, CurrentUser(tk.IdTaiKhoan));

        var act = async () => await handler.Handle(
            new DanhSachLichHenCuaToiQuery(null, 1, 20), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
