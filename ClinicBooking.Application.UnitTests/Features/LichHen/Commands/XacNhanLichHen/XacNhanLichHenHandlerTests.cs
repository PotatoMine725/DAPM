using ClinicBooking.Application.Abstractions.Notifications;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.LichHen.Commands.XacNhanLichHen;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace ClinicBooking.Application.UnitTests.Features.LichHen.Commands.XacNhanLichHen;

public sealed class XacNhanLichHenHandlerTests
{
    private static readonly DateTime FixedNow = new(2026, 5, 1, 10, 0, 0, DateTimeKind.Utc);

    private static (ICurrentUserService user, IDateTimeProvider clock, INotificationService notif) CreateDeps()
    {
        var user = Substitute.For<ICurrentUserService>();
        user.IdTaiKhoan.Returns(1);
        user.VaiTro.Returns(VaiTro.LeTan);
        var clock = Substitute.For<IDateTimeProvider>();
        clock.UtcNow.Returns(FixedNow);
        var notif = Substitute.For<INotificationService>();
        return (user, clock, notif);
    }

    [Fact]
    public async Task Handle_LichChoXacNhan_ChuyenSangDaXacNhan_GhiLichSu_GuiThongBao()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var bn = TestDataSeeder.SeedBenhNhan(db);
        var ca = TestDataSeeder.SeedCaLamViec(db);
        var lh = TestDataSeeder.SeedLichHen(db, bn.IdBenhNhan, ca.IdCaLamViec, trangThai: TrangThaiLichHen.ChoXacNhan);
        var (user, clock, notif) = CreateDeps();

        var handler = new XacNhanLichHenHandler(db, user, clock, notif);
        await handler.Handle(new XacNhanLichHenCommand(lh.IdLichHen), CancellationToken.None);

        var sau = await db.LichHen.AsNoTracking().FirstAsync(x => x.IdLichHen == lh.IdLichHen);
        sau.TrangThai.Should().Be(TrangThaiLichHen.DaXacNhan);
        (await db.LichSuLichHen.AsNoTracking().CountAsync(x => x.IdLichHen == lh.IdLichHen)).Should().Be(1);
        await notif.Received(1).GuiThongBaoXacNhanLichHenAsync(lh.IdLichHen, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_LichHenKhongTonTai_ThrowNotFoundException()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var (user, clock, notif) = CreateDeps();
        var handler = new XacNhanLichHenHandler(db, user, clock, notif);

        var act = async () => await handler.Handle(new XacNhanLichHenCommand(999), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_LichDaXacNhan_ThrowConflictException()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var bn = TestDataSeeder.SeedBenhNhan(db);
        var ca = TestDataSeeder.SeedCaLamViec(db);
        var lh = TestDataSeeder.SeedLichHen(db, bn.IdBenhNhan, ca.IdCaLamViec, trangThai: TrangThaiLichHen.DaXacNhan);
        var (user, clock, notif) = CreateDeps();

        var handler = new XacNhanLichHenHandler(db, user, clock, notif);
        var act = async () => await handler.Handle(new XacNhanLichHenCommand(lh.IdLichHen), CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("Chi co the xac nhan lich hen dang cho xac nhan.");
    }
}
