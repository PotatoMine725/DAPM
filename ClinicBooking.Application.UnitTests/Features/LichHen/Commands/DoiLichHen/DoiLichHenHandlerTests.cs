using ClinicBooking.Application.Abstractions.Notifications;
using ClinicBooking.Application.Abstractions.Scheduling;
using ClinicBooking.Application.Abstractions.Scheduling.Dtos;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Common.Services;
using ClinicBooking.Application.Features.LichHen.Commands.DoiLichHen;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace ClinicBooking.Application.UnitTests.Features.LichHen.Commands.DoiLichHen;

public sealed class DoiLichHenHandlerTests
{
    private static readonly DateTime FixedNow = new(2026, 5, 1, 10, 0, 0, DateTimeKind.Utc);

    private sealed record Deps(
        ICurrentUserService User,
        IDateTimeProvider Clock,
        ICaLamViecQueryService Scheduling,
        INotificationService Notif,
        IMaLichHenGenerator MaGen);

    private static Deps CreateDeps(VaiTro vaiTro, int idTaiKhoan = 1)
    {
        var user = Substitute.For<ICurrentUserService>();
        user.VaiTro.Returns(vaiTro);
        user.IdTaiKhoan.Returns(idTaiKhoan);
        var clock = Substitute.For<IDateTimeProvider>();
        clock.UtcNow.Returns(FixedNow);
        var scheduling = Substitute.For<ICaLamViecQueryService>();
        var notif = Substitute.For<INotificationService>();
        var maGen = Substitute.For<IMaLichHenGenerator>();
        maGen.SinhMaLichHenAsync(Arg.Any<DateOnly>(), Arg.Any<CancellationToken>()).Returns("LH-20260510-000001");
        return new Deps(user, clock, scheduling, notif, maGen);
    }

    private static ThongTinCaLamViecDto ThongTinOk(int idCa, DateOnly? ngay = null) => new(
        idCa, 1, 1, 1, 1,
        ngay ?? new DateOnly(2026, 5, 10), new TimeOnly(8, 0), new TimeOnly(11, 0),
        15, 10, 0, TrangThaiDuyetCa.DaDuyet);

    [Fact]
    public async Task Handle_LeTanDoiLich_TaoLichMoi_HuyLichCu_GhiLichSu()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var bn = TestDataSeeder.SeedBenhNhan(db);
        var caCu = TestDataSeeder.SeedCaLamViec(db);
        var caMoi = TestDataSeeder.SeedCaLamViec(db, ngayLamViec: new DateOnly(2026, 5, 10));
        var lhCu = TestDataSeeder.SeedLichHen(db, bn.IdBenhNhan, caCu.IdCaLamViec, trangThai: TrangThaiLichHen.ChoXacNhan);
        var d = CreateDeps(VaiTro.LeTan);
        d.Scheduling.LayThongTinCaAsync(caMoi.IdCaLamViec, Arg.Any<CancellationToken>()).Returns(ThongTinOk(caMoi.IdCaLamViec));
        d.Scheduling.KiemTraSlotTrongAsync(caMoi.IdCaLamViec, Arg.Any<CancellationToken>())
            .Returns(new KetQuaKiemTraSlotDto(true, 10, 0, 0, null));
        d.Scheduling.IncrementSoSlotDaDatAsync(caMoi.IdCaLamViec, 1, Arg.Any<CancellationToken>()).Returns((int?)1);
        d.Scheduling.IncrementSoSlotDaDatAsync(caCu.IdCaLamViec, -1, Arg.Any<CancellationToken>()).Returns((int?)0);

        var handler = new DoiLichHenHandler(db, d.User, d.Clock, d.Scheduling, d.Notif, d.MaGen);
        var result = await handler.Handle(
            new DoiLichHenCommand(lhCu.IdLichHen, caMoi.IdCaLamViec, null, null, null, null, "Can doi"),
            CancellationToken.None);

        result.IdLichHen.Should().NotBe(lhCu.IdLichHen);
        result.IdCaLamViec.Should().Be(caMoi.IdCaLamViec);
        var lhCuSau = await db.LichHen.AsNoTracking().FirstAsync(x => x.IdLichHen == lhCu.IdLichHen);
        lhCuSau.TrangThai.Should().Be(TrangThaiLichHen.HuyPhongKham);
        (await db.LichSuLichHen.AsNoTracking().AnyAsync(x => x.IdLichHen == lhCu.IdLichHen && x.HanhDong == HanhDongLichSu.DoiLich)).Should().BeTrue();
        (await db.LichSuLichHen.AsNoTracking().AnyAsync(x => x.IdLichHen == result.IdLichHen && x.HanhDong == HanhDongLichSu.DatMoi && x.IdLichHenTruoc == lhCu.IdLichHen)).Should().BeTrue();
        await d.Notif.Received(1).GuiThongBaoDoiLichHenAsync(lhCu.IdLichHen, result.IdLichHen, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CaMoiTrungCaCu_ThrowConflict()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var bn = TestDataSeeder.SeedBenhNhan(db);
        var ca = TestDataSeeder.SeedCaLamViec(db);
        var lh = TestDataSeeder.SeedLichHen(db, bn.IdBenhNhan, ca.IdCaLamViec, trangThai: TrangThaiLichHen.ChoXacNhan);
        var d = CreateDeps(VaiTro.LeTan);

        var handler = new DoiLichHenHandler(db, d.User, d.Clock, d.Scheduling, d.Notif, d.MaGen);
        var act = async () => await handler.Handle(
            new DoiLichHenCommand(lh.IdLichHen, ca.IdCaLamViec, null, null, null, null, null),
            CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>().WithMessage("Ca moi phai khac ca hien tai.");
    }

    [Fact]
    public async Task Handle_LichDaHoanThanh_ThrowConflict()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var bn = TestDataSeeder.SeedBenhNhan(db);
        var caCu = TestDataSeeder.SeedCaLamViec(db);
        var caMoi = TestDataSeeder.SeedCaLamViec(db);
        var lh = TestDataSeeder.SeedLichHen(db, bn.IdBenhNhan, caCu.IdCaLamViec, trangThai: TrangThaiLichHen.HoanThanh);
        var d = CreateDeps(VaiTro.LeTan);

        var handler = new DoiLichHenHandler(db, d.User, d.Clock, d.Scheduling, d.Notif, d.MaGen);
        var act = async () => await handler.Handle(
            new DoiLichHenCommand(lh.IdLichHen, caMoi.IdCaLamViec, null, null, null, null, null),
            CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task Handle_HetSlotCaMoi_IncrementNull_ThrowConflict()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var bn = TestDataSeeder.SeedBenhNhan(db);
        var caCu = TestDataSeeder.SeedCaLamViec(db);
        var caMoi = TestDataSeeder.SeedCaLamViec(db);
        var lh = TestDataSeeder.SeedLichHen(db, bn.IdBenhNhan, caCu.IdCaLamViec, trangThai: TrangThaiLichHen.ChoXacNhan);
        var d = CreateDeps(VaiTro.LeTan);
        d.Scheduling.LayThongTinCaAsync(caMoi.IdCaLamViec, Arg.Any<CancellationToken>()).Returns(ThongTinOk(caMoi.IdCaLamViec));
        d.Scheduling.KiemTraSlotTrongAsync(caMoi.IdCaLamViec, Arg.Any<CancellationToken>())
            .Returns(new KetQuaKiemTraSlotDto(true, 10, 0, 0, null));
        d.Scheduling.IncrementSoSlotDaDatAsync(caMoi.IdCaLamViec, 1, Arg.Any<CancellationToken>()).Returns((int?)null);

        var handler = new DoiLichHenHandler(db, d.User, d.Clock, d.Scheduling, d.Notif, d.MaGen);
        var act = async () => await handler.Handle(
            new DoiLichHenCommand(lh.IdLichHen, caMoi.IdCaLamViec, null, null, null, null, null),
            CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>().WithMessage("Ca lam viec moi da het slot.");
        var lhCuSau = await db.LichHen.AsNoTracking().FirstAsync(x => x.IdLichHen == lh.IdLichHen);
        lhCuSau.TrangThai.Should().Be(TrangThaiLichHen.ChoXacNhan);
        await d.Notif.DidNotReceive().GuiThongBaoDoiLichHenAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_KhongGiaiPhongDuocSlotCaCu_ThrowConflict()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var bn = TestDataSeeder.SeedBenhNhan(db);
        var caCu = TestDataSeeder.SeedCaLamViec(db);
        var caMoi = TestDataSeeder.SeedCaLamViec(db);
        var lh = TestDataSeeder.SeedLichHen(db, bn.IdBenhNhan, caCu.IdCaLamViec, trangThai: TrangThaiLichHen.ChoXacNhan);
        var d = CreateDeps(VaiTro.LeTan);
        d.Scheduling.LayThongTinCaAsync(caMoi.IdCaLamViec, Arg.Any<CancellationToken>()).Returns(ThongTinOk(caMoi.IdCaLamViec));
        d.Scheduling.KiemTraSlotTrongAsync(caMoi.IdCaLamViec, Arg.Any<CancellationToken>())
            .Returns(new KetQuaKiemTraSlotDto(true, 10, 0, 0, null));
        d.Scheduling.IncrementSoSlotDaDatAsync(caMoi.IdCaLamViec, 1, Arg.Any<CancellationToken>()).Returns((int?)1);
        d.Scheduling.IncrementSoSlotDaDatAsync(caCu.IdCaLamViec, -1, Arg.Any<CancellationToken>()).Returns((int?)null);

        var handler = new DoiLichHenHandler(db, d.User, d.Clock, d.Scheduling, d.Notif, d.MaGen);
        var act = async () => await handler.Handle(
            new DoiLichHenCommand(lh.IdLichHen, caMoi.IdCaLamViec, null, null, null, null, null),
            CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>().WithMessage("Khong the giai phong slot cua ca cu.");
        await d.Notif.DidNotReceive().GuiThongBaoDoiLichHenAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
    }
}
