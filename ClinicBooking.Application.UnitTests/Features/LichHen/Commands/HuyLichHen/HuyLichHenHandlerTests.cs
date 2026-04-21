using ClinicBooking.Application.Abstractions.Notifications;
using ClinicBooking.Application.Abstractions.Scheduling;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Common.Options;
using ClinicBooking.Application.Features.LichHen.Commands.HuyLichHen;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace ClinicBooking.Application.UnitTests.Features.LichHen.Commands.HuyLichHen;

public sealed class HuyLichHenHandlerTests
{
    private static IOptions<LichHenOptions> DefaultOptions() =>
        Options.Create(new LichHenOptions
        {
            HuyMuonTruocGio = 24,
            GiuChoThoiHanPhut = 15,
            MaLichHenPrefix = "LH"
        });

    private static (ICurrentUserService user, IDateTimeProvider clock, ICaLamViecQueryService scheduling, INotificationService notif)
        CreateDeps(VaiTro vaiTro, int? idTaiKhoan = 1, DateTime? now = null)
    {
        var user = Substitute.For<ICurrentUserService>();
        user.VaiTro.Returns(vaiTro);
        user.IdTaiKhoan.Returns(idTaiKhoan);
        var clock = Substitute.For<IDateTimeProvider>();
        clock.UtcNow.Returns(now ?? new DateTime(2026, 5, 1, 10, 0, 0, DateTimeKind.Utc));
        var scheduling = Substitute.For<ICaLamViecQueryService>();
        scheduling.IncrementSoSlotDaDatAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns((int?)0);
        var notif = Substitute.For<INotificationService>();
        return (user, clock, scheduling, notif);
    }

    [Fact]
    public async Task Handle_LeTanHuy_GhiHuyPhongKham_VaGuiThongBao()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        // Ca lam viec cach hien tai > 24h (2026-05-05 vs now 2026-05-01) -> khong huy muon.
        var bn = TestDataSeeder.SeedBenhNhan(db);
        var ca = TestDataSeeder.SeedCaLamViec(db);
        var lh = TestDataSeeder.SeedLichHen(db, bn.IdBenhNhan, ca.IdCaLamViec, trangThai: TrangThaiLichHen.DaXacNhan);
        var (user, clock, scheduling, notif) = CreateDeps(VaiTro.LeTan);

        var handler = new HuyLichHenHandler(db, user, clock, scheduling, notif, DefaultOptions());
        await handler.Handle(new HuyLichHenCommand(lh.IdLichHen, "Benh nhan yeu cau"), CancellationToken.None);

        var sau = await db.LichHen.AsNoTracking().FirstAsync(x => x.IdLichHen == lh.IdLichHen);
        sau.TrangThai.Should().Be(TrangThaiLichHen.HuyPhongKham);
        var lichSu = await db.LichSuLichHen.AsNoTracking().FirstAsync(x => x.IdLichHen == lh.IdLichHen);
        lichSu.HanhDong.Should().Be(HanhDongLichSu.HuyPhongKham);
        lichSu.DanhDauHuyMuon.Should().BeFalse();
        await scheduling.Received(1).IncrementSoSlotDaDatAsync(ca.IdCaLamViec, -1, Arg.Any<CancellationToken>());
        await notif.Received(1).GuiThongBaoHuyLichHenAsync(lh.IdLichHen, "Benh nhan yeu cau", true, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_BenhNhanHuyMuon_TangSoLanHuyMuon_DanhDau()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var tk = TestDataSeeder.SeedTaiKhoan(db, VaiTro.BenhNhan);
        var bn = TestDataSeeder.SeedBenhNhan(db, idTaiKhoan: tk.IdTaiKhoan);
        // Ca lam viec 2026-05-05 08:00, now 2026-05-05 00:00 -> < 24h -> huy muon.
        var ca = TestDataSeeder.SeedCaLamViec(db, ngayLamViec: new DateOnly(2026, 5, 5), gioBatDau: new TimeOnly(8, 0));
        var lh = TestDataSeeder.SeedLichHen(db, bn.IdBenhNhan, ca.IdCaLamViec, trangThai: TrangThaiLichHen.DaXacNhan);
        var (user, clock, scheduling, notif) = CreateDeps(VaiTro.BenhNhan, idTaiKhoan: tk.IdTaiKhoan,
            now: new DateTime(2026, 5, 5, 0, 0, 0, DateTimeKind.Utc));

        var handler = new HuyLichHenHandler(db, user, clock, scheduling, notif, DefaultOptions());
        await handler.Handle(new HuyLichHenCommand(lh.IdLichHen, "Ban viec"), CancellationToken.None);

        var sau = await db.LichHen.AsNoTracking().FirstAsync(x => x.IdLichHen == lh.IdLichHen);
        sau.TrangThai.Should().Be(TrangThaiLichHen.HuyBenhNhan);
        var bnSau = await db.BenhNhan.AsNoTracking().FirstAsync(x => x.IdBenhNhan == bn.IdBenhNhan);
        bnSau.SoLanHuyMuon.Should().Be(1);
        var lichSu = await db.LichSuLichHen.AsNoTracking().FirstAsync(x => x.IdLichHen == lh.IdLichHen);
        lichSu.DanhDauHuyMuon.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_BenhNhanKhongPhaiChuSoHuu_ThrowForbidden()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var tkChu = TestDataSeeder.SeedTaiKhoan(db, VaiTro.BenhNhan);
        var bn = TestDataSeeder.SeedBenhNhan(db, idTaiKhoan: tkChu.IdTaiKhoan);
        var ca = TestDataSeeder.SeedCaLamViec(db);
        var lh = TestDataSeeder.SeedLichHen(db, bn.IdBenhNhan, ca.IdCaLamViec, trangThai: TrangThaiLichHen.DaXacNhan);
        var tkNguoiKhac = TestDataSeeder.SeedTaiKhoan(db, VaiTro.BenhNhan);
        var (user, clock, scheduling, notif) = CreateDeps(VaiTro.BenhNhan, idTaiKhoan: tkNguoiKhac.IdTaiKhoan);

        var handler = new HuyLichHenHandler(db, user, clock, scheduling, notif, DefaultOptions());
        var act = async () => await handler.Handle(new HuyLichHenCommand(lh.IdLichHen, "x"), CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task Handle_BacSiHuy_ThrowForbidden()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var bn = TestDataSeeder.SeedBenhNhan(db);
        var ca = TestDataSeeder.SeedCaLamViec(db);
        var lh = TestDataSeeder.SeedLichHen(db, bn.IdBenhNhan, ca.IdCaLamViec, trangThai: TrangThaiLichHen.DaXacNhan);
        var (user, clock, scheduling, notif) = CreateDeps(VaiTro.BacSi);

        var handler = new HuyLichHenHandler(db, user, clock, scheduling, notif, DefaultOptions());
        var act = async () => await handler.Handle(new HuyLichHenCommand(lh.IdLichHen, "x"), CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task Handle_LichDaHoanThanh_ThrowConflict()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var bn = TestDataSeeder.SeedBenhNhan(db);
        var ca = TestDataSeeder.SeedCaLamViec(db);
        var lh = TestDataSeeder.SeedLichHen(db, bn.IdBenhNhan, ca.IdCaLamViec, trangThai: TrangThaiLichHen.HoanThanh);
        var (user, clock, scheduling, notif) = CreateDeps(VaiTro.LeTan);

        var handler = new HuyLichHenHandler(db, user, clock, scheduling, notif, DefaultOptions());
        var act = async () => await handler.Handle(new HuyLichHenCommand(lh.IdLichHen, "x"), CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task Handle_LichHenKhongTonTai_ThrowNotFound()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var (user, clock, scheduling, notif) = CreateDeps(VaiTro.LeTan);

        var handler = new HuyLichHenHandler(db, user, clock, scheduling, notif, DefaultOptions());
        var act = async () => await handler.Handle(new HuyLichHenCommand(999, "x"), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
