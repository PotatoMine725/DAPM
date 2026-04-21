using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.LichHen.Queries.XemLichHen;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Enums;
using FluentAssertions;
using NSubstitute;

namespace ClinicBooking.Application.UnitTests.Features.LichHen.Queries.XemLichHen;

public sealed class XemLichHenHandlerTests
{
    private static ICurrentUserService CurrentUser(VaiTro vaiTro, int? idTaiKhoan)
    {
        var cu = Substitute.For<ICurrentUserService>();
        cu.VaiTro.Returns(vaiTro);
        cu.IdTaiKhoan.Returns(idTaiKhoan);
        return cu;
    }

    [Fact]
    public async Task Handle_BenhNhanLaChuSoHuu_TraVeLichHen()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var tk = TestDataSeeder.SeedTaiKhoan(db, VaiTro.BenhNhan);
        var bn = TestDataSeeder.SeedBenhNhan(db, tk.IdTaiKhoan);
        var ca = TestDataSeeder.SeedCaLamViec(db);
        var lh = TestDataSeeder.SeedLichHen(db, bn.IdBenhNhan, ca.IdCaLamViec);

        var handler = new XemLichHenHandler(db, CurrentUser(VaiTro.BenhNhan, tk.IdTaiKhoan));
        var result = await handler.Handle(new XemLichHenQuery(lh.IdLichHen), CancellationToken.None);

        result.IdLichHen.Should().Be(lh.IdLichHen);
        result.HoTenBenhNhan.Should().Be(bn.HoTen);
    }

    [Fact]
    public async Task Handle_LeTan_XemDuocMoiLichHen()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var tkLeTan = TestDataSeeder.SeedTaiKhoan(db, VaiTro.LeTan);
        var bn = TestDataSeeder.SeedBenhNhan(db);
        var ca = TestDataSeeder.SeedCaLamViec(db);
        var lh = TestDataSeeder.SeedLichHen(db, bn.IdBenhNhan, ca.IdCaLamViec);

        var handler = new XemLichHenHandler(db, CurrentUser(VaiTro.LeTan, tkLeTan.IdTaiKhoan));
        var result = await handler.Handle(new XemLichHenQuery(lh.IdLichHen), CancellationToken.None);

        result.IdLichHen.Should().Be(lh.IdLichHen);
    }

    [Fact]
    public async Task Handle_BenhNhanKhongPhaiChuSoHuu_ThrowForbiddenException()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var tkKhac = TestDataSeeder.SeedTaiKhoan(db, VaiTro.BenhNhan);
        var bnKhac = TestDataSeeder.SeedBenhNhan(db, tkKhac.IdTaiKhoan);

        var tkChuSo = TestDataSeeder.SeedTaiKhoan(db, VaiTro.BenhNhan);
        var bnChuSo = TestDataSeeder.SeedBenhNhan(db, tkChuSo.IdTaiKhoan);
        var ca = TestDataSeeder.SeedCaLamViec(db);
        var lh = TestDataSeeder.SeedLichHen(db, bnChuSo.IdBenhNhan, ca.IdCaLamViec);

        var handler = new XemLichHenHandler(db, CurrentUser(VaiTro.BenhNhan, tkKhac.IdTaiKhoan));
        var act = async () => await handler.Handle(new XemLichHenQuery(lh.IdLichHen), CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>()
            .WithMessage("Ban khong co quyen xem lich hen nay.");
    }

    [Fact]
    public async Task Handle_LichHenKhongTonTai_ThrowNotFoundException()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var handler = new XemLichHenHandler(db, CurrentUser(VaiTro.LeTan, 1));

        var act = async () => await handler.Handle(new XemLichHenQuery(999), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Khong tim thay lich hen.");
    }
}
