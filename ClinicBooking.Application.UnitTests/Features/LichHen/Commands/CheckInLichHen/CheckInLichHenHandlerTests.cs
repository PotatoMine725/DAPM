using ClinicBooking.Application.Abstractions.Notifications;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.LichHen.Commands.CheckInLichHen;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace ClinicBooking.Application.UnitTests.Features.LichHen.Commands.CheckInLichHen;

public sealed class CheckInLichHenHandlerTests
{
    private static (ICurrentUserService user, IDateTimeProvider clock, INotificationService notif) CreateDeps()
    {
        var user = Substitute.For<ICurrentUserService>();
        user.IdTaiKhoan.Returns(1);
        user.VaiTro.Returns(VaiTro.LeTan);
        var clock = Substitute.For<IDateTimeProvider>();
        clock.UtcNow.Returns(new DateTime(2026, 5, 1, 8, 0, 0, DateTimeKind.Utc));
        var notif = Substitute.For<INotificationService>();
        return (user, clock, notif);
    }

    [Fact]
    public async Task Handle_LichDaXacNhan_TaoHangCho_VoiSoThuTuKeTiep()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var bn = TestDataSeeder.SeedBenhNhan(db);
        var ca = TestDataSeeder.SeedCaLamViec(db);
        var lh = TestDataSeeder.SeedLichHen(db, bn.IdBenhNhan, ca.IdCaLamViec, trangThai: TrangThaiLichHen.DaXacNhan);
        // Co 1 hang cho san -> MAX = 3, can tao SoThuTu = 4.
        var bn2 = TestDataSeeder.SeedBenhNhan(db);
        var lhKhac = TestDataSeeder.SeedLichHen(db, bn2.IdBenhNhan, ca.IdCaLamViec, soSlot: 2, trangThai: TrangThaiLichHen.DaXacNhan);
        db.HangCho.Add(new ClinicBooking.Domain.Entities.HangCho
        {
            IdCaLamViec = ca.IdCaLamViec,
            IdLichHen = lhKhac.IdLichHen,
            SoThuTu = 3,
            TrangThai = TrangThaiHangCho.ChoKham,
            NgayCheckIn = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        var (user, clock, notif) = CreateDeps();
        var handler = new CheckInLichHenHandler(db, user, clock, notif);
        var result = await handler.Handle(new CheckInLichHenCommand(lh.IdLichHen), CancellationToken.None);

        result.SoThuTu.Should().Be(4);
        result.TrangThai.Should().Be(TrangThaiHangCho.ChoKham);
        (await db.HangCho.AsNoTracking().CountAsync(x => x.IdCaLamViec == ca.IdCaLamViec)).Should().Be(2);
        (await db.LichSuLichHen.AsNoTracking().AnyAsync(x => x.IdLichHen == lh.IdLichHen && x.HanhDong == HanhDongLichSu.CheckIn)).Should().BeTrue();
        await notif.Received(1).GuiThongBaoCheckInAsync(result.IdHangCho, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_LichChuaXacNhan_ThrowConflict()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var bn = TestDataSeeder.SeedBenhNhan(db);
        var ca = TestDataSeeder.SeedCaLamViec(db);
        var lh = TestDataSeeder.SeedLichHen(db, bn.IdBenhNhan, ca.IdCaLamViec, trangThai: TrangThaiLichHen.ChoXacNhan);
        var (user, clock, notif) = CreateDeps();

        var handler = new CheckInLichHenHandler(db, user, clock, notif);
        var act = async () => await handler.Handle(new CheckInLichHenCommand(lh.IdLichHen), CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>().WithMessage("Chi co the check-in lich da xac nhan.");
    }

    [Fact]
    public async Task Handle_DaCheckInTruoc_ThrowConflict()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var bn = TestDataSeeder.SeedBenhNhan(db);
        var ca = TestDataSeeder.SeedCaLamViec(db);
        var lh = TestDataSeeder.SeedLichHen(db, bn.IdBenhNhan, ca.IdCaLamViec, trangThai: TrangThaiLichHen.DaXacNhan);
        db.HangCho.Add(new ClinicBooking.Domain.Entities.HangCho
        {
            IdCaLamViec = ca.IdCaLamViec,
            IdLichHen = lh.IdLichHen,
            SoThuTu = 1,
            TrangThai = TrangThaiHangCho.ChoKham,
            NgayCheckIn = DateTime.UtcNow
        });
        await db.SaveChangesAsync();
        var (user, clock, notif) = CreateDeps();

        var handler = new CheckInLichHenHandler(db, user, clock, notif);
        var act = async () => await handler.Handle(new CheckInLichHenCommand(lh.IdLichHen), CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>().WithMessage("Lich hen da check-in truoc do.");
    }

    [Fact]
    public async Task Handle_LichHenKhongTonTai_ThrowNotFound()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var (user, clock, notif) = CreateDeps();

        var handler = new CheckInLichHenHandler(db, user, clock, notif);
        var act = async () => await handler.Handle(new CheckInLichHenCommand(999), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
