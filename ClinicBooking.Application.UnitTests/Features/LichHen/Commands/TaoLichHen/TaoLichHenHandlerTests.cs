using ClinicBooking.Application.Abstractions.Notifications;
using ClinicBooking.Application.Abstractions.Scheduling;
using ClinicBooking.Application.Abstractions.Scheduling.Dtos;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Common.Services;
using ClinicBooking.Application.Features.LichHen.Commands.TaoLichHen;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Enums;
using ClinicBooking.Infrastructure.Services.Scheduling;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace ClinicBooking.Application.UnitTests.Features.LichHen.Commands.TaoLichHen;

public sealed class TaoLichHenHandlerTests
{
    private static readonly DateTime FixedNow = new(2026, 5, 1, 10, 0, 0, DateTimeKind.Utc);

    private sealed record Deps(
        ICurrentUserService User,
        IDateTimeProvider Clock,
        ICaLamViecQueryService Scheduling,
        INotificationService Notif,
        IMaLichHenGenerator MaGen);

    private static Deps CreateDeps(VaiTro vaiTro, int? idTaiKhoan = 1)
    {
        var user = Substitute.For<ICurrentUserService>();
        user.VaiTro.Returns(vaiTro);
        user.IdTaiKhoan.Returns(idTaiKhoan);
        var clock = Substitute.For<IDateTimeProvider>();
        clock.UtcNow.Returns(FixedNow);
        var scheduling = Substitute.For<ICaLamViecQueryService>();
        var notif = Substitute.For<INotificationService>();
        var maGen = Substitute.For<IMaLichHenGenerator>();
        maGen.SinhMaLichHenAsync(Arg.Any<DateOnly>(), Arg.Any<CancellationToken>())
            .Returns("LH-20260505-000001");
        return new Deps(user, clock, scheduling, notif, maGen);
    }

    private static ThongTinCaLamViecDto ThongTinCaOk(int idCa) => new(
        IdCaLamViec: idCa,
        IdBacSi: 1,
        IdPhong: 1,
        IdChuyenKhoa: 1,
        IdDinhNghiaCa: 1,
        NgayLamViec: new DateOnly(2026, 5, 5),
        GioBatDau: new TimeOnly(8, 0),
        GioKetThuc: new TimeOnly(11, 0),
        ThoiGianSlot: 15,
        SoSlotToiDa: 10,
        SoSlotDaDat: 0,
        TrangThaiDuyet: TrangThaiDuyetCa.DaDuyet);

    [Fact]
    public async Task Handle_BenhNhanDatLich_TaoLichHen_SinhMa_GuiThongBao()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var tk = TestDataSeeder.SeedTaiKhoan(db, VaiTro.BenhNhan);
        var bn = TestDataSeeder.SeedBenhNhan(db, idTaiKhoan: tk.IdTaiKhoan);
        var ca = TestDataSeeder.SeedCaLamViec(db);
        var dv = TestDataSeeder.SeedDichVu(db);
        var d = CreateDeps(VaiTro.BenhNhan, idTaiKhoan: tk.IdTaiKhoan);
        d.Scheduling.LayThongTinCaAsync(ca.IdCaLamViec, Arg.Any<CancellationToken>())
            .Returns(ThongTinCaOk(ca.IdCaLamViec));
        d.Scheduling.KiemTraSlotTrongAsync(ca.IdCaLamViec, Arg.Any<CancellationToken>())
            .Returns(new KetQuaKiemTraSlotDto(true, 10, 0, 0, null));
        d.Scheduling.IncrementSoSlotDaDatAsync(ca.IdCaLamViec, 1, Arg.Any<CancellationToken>())
            .Returns((int?)1);

        var handler = new TaoLichHenHandler(db, d.User, d.Clock, d.Scheduling, d.Notif, d.MaGen);
        var result = await handler.Handle(
            new TaoLichHenCommand(ca.IdCaLamViec, dv.IdDichVu, null, null, null, "Dau dau"),
            CancellationToken.None);

        result.MaLichHen.Should().Be("LH-20260505-000001");
        result.SoSlot.Should().Be(1);
        result.IdBenhNhan.Should().Be(bn.IdBenhNhan);
        result.TrangThai.Should().Be(TrangThaiLichHen.ChoXacNhan);
        result.HinhThucDat.Should().Be(HinhThucDat.TrucTuyen);
        (await db.LichSuLichHen.AsNoTracking().AnyAsync(x => x.IdLichHen == result.IdLichHen && x.HanhDong == HanhDongLichSu.DatMoi))
            .Should().BeTrue();
        await d.Notif.Received(1).GuiThongBaoTaoLichHenAsync(result.IdLichHen, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_LeTanDatHo_YeuCauIdBenhNhan()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var ca = TestDataSeeder.SeedCaLamViec(db);
        var dv = TestDataSeeder.SeedDichVu(db);
        var d = CreateDeps(VaiTro.LeTan);

        var handler = new TaoLichHenHandler(db, d.User, d.Clock, d.Scheduling, d.Notif, d.MaGen);
        var act = async () => await handler.Handle(
            new TaoLichHenCommand(ca.IdCaLamViec, dv.IdDichVu, null, null, null, null),
            CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>().WithMessage("Le tan phai chon benh nhan de dat lich.");
    }

    [Fact]
    public async Task Handle_BenhNhanBiHanChe_ThrowConflict()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var tk = TestDataSeeder.SeedTaiKhoan(db, VaiTro.BenhNhan);
        var bn = TestDataSeeder.SeedBenhNhan(db, idTaiKhoan: tk.IdTaiKhoan, biHanChe: true);
        var ca = TestDataSeeder.SeedCaLamViec(db);
        var dv = TestDataSeeder.SeedDichVu(db);
        var d = CreateDeps(VaiTro.BenhNhan, idTaiKhoan: tk.IdTaiKhoan);

        var handler = new TaoLichHenHandler(db, d.User, d.Clock, d.Scheduling, d.Notif, d.MaGen);
        var act = async () => await handler.Handle(
            new TaoLichHenCommand(ca.IdCaLamViec, dv.IdDichVu, null, null, null, null),
            CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task Handle_CaChuaDuyet_ThrowConflict()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var tk = TestDataSeeder.SeedTaiKhoan(db, VaiTro.BenhNhan);
        var bn = TestDataSeeder.SeedBenhNhan(db, idTaiKhoan: tk.IdTaiKhoan);
        var ca = TestDataSeeder.SeedCaLamViec(db);
        var dv = TestDataSeeder.SeedDichVu(db);
        var d = CreateDeps(VaiTro.BenhNhan, idTaiKhoan: tk.IdTaiKhoan);
        d.Scheduling.LayThongTinCaAsync(ca.IdCaLamViec, Arg.Any<CancellationToken>())
            .Returns(ThongTinCaOk(ca.IdCaLamViec) with { TrangThaiDuyet = TrangThaiDuyetCa.ChoDuyet });

        var handler = new TaoLichHenHandler(db, d.User, d.Clock, d.Scheduling, d.Notif, d.MaGen);
        var act = async () => await handler.Handle(
            new TaoLichHenCommand(ca.IdCaLamViec, dv.IdDichVu, null, null, null, null),
            CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>().WithMessage("Ca lam viec chua duoc duyet.");
    }

    [Fact]
    public async Task Handle_HetSlot_IncrementTraVeNull_ThrowConflict()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var tk = TestDataSeeder.SeedTaiKhoan(db, VaiTro.BenhNhan);
        var bn = TestDataSeeder.SeedBenhNhan(db, idTaiKhoan: tk.IdTaiKhoan);
        var ca = TestDataSeeder.SeedCaLamViec(db);
        var dv = TestDataSeeder.SeedDichVu(db);
        var d = CreateDeps(VaiTro.BenhNhan, idTaiKhoan: tk.IdTaiKhoan);
        d.Scheduling.LayThongTinCaAsync(ca.IdCaLamViec, Arg.Any<CancellationToken>()).Returns(ThongTinCaOk(ca.IdCaLamViec));
        d.Scheduling.KiemTraSlotTrongAsync(ca.IdCaLamViec, Arg.Any<CancellationToken>())
            .Returns(new KetQuaKiemTraSlotDto(true, 10, 0, 0, null));
        d.Scheduling.IncrementSoSlotDaDatAsync(ca.IdCaLamViec, 1, Arg.Any<CancellationToken>())
            .Returns((int?)null);

        var handler = new TaoLichHenHandler(db, d.User, d.Clock, d.Scheduling, d.Notif, d.MaGen);
        var act = async () => await handler.Handle(
            new TaoLichHenCommand(ca.IdCaLamViec, dv.IdDichVu, null, null, null, null),
            CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>().WithMessage("Ca lam viec da het slot.");
    }

    [Fact]
    public async Task Handle_HaiBenhNhanDatDongThoi_ChiMotNguoiDatDuoc()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var tk1 = TestDataSeeder.SeedTaiKhoan(db, VaiTro.BenhNhan);
        var bn1 = TestDataSeeder.SeedBenhNhan(db, idTaiKhoan: tk1.IdTaiKhoan);
        var tk2 = TestDataSeeder.SeedTaiKhoan(db, VaiTro.BenhNhan);
        var bn2 = TestDataSeeder.SeedBenhNhan(db, idTaiKhoan: tk2.IdTaiKhoan);
        var ca = TestDataSeeder.SeedCaLamViec(db);
        var dv = TestDataSeeder.SeedDichVu(db);

        var d1 = CreateDeps(VaiTro.BenhNhan, idTaiKhoan: tk1.IdTaiKhoan);
        var d2 = CreateDeps(VaiTro.BenhNhan, idTaiKhoan: tk2.IdTaiKhoan);

        var thongTin = ThongTinCaOk(ca.IdCaLamViec);
        d1.Scheduling.LayThongTinCaAsync(ca.IdCaLamViec, Arg.Any<CancellationToken>()).Returns(thongTin);
        d2.Scheduling.LayThongTinCaAsync(ca.IdCaLamViec, Arg.Any<CancellationToken>()).Returns(thongTin);
        d1.Scheduling.KiemTraSlotTrongAsync(ca.IdCaLamViec, Arg.Any<CancellationToken>()).Returns(new KetQuaKiemTraSlotDto(true, 10, 0, 0, null));
        d2.Scheduling.KiemTraSlotTrongAsync(ca.IdCaLamViec, Arg.Any<CancellationToken>()).Returns(new KetQuaKiemTraSlotDto(true, 10, 0, 0, null));
        d1.Scheduling.IncrementSoSlotDaDatAsync(ca.IdCaLamViec, 1, Arg.Any<CancellationToken>()).Returns((int?)1);
        d2.Scheduling.IncrementSoSlotDaDatAsync(ca.IdCaLamViec, 1, Arg.Any<CancellationToken>()).Returns((int?)null);

        var handler1 = new TaoLichHenHandler(db, d1.User, d1.Clock, d1.Scheduling, d1.Notif, d1.MaGen);
        var handler2 = new TaoLichHenHandler(db, d2.User, d2.Clock, d2.Scheduling, d2.Notif, d2.MaGen);

        var task1 = handler1.Handle(new TaoLichHenCommand(ca.IdCaLamViec, dv.IdDichVu, null, null, null, "A"), CancellationToken.None);
        var task2 = handler2.Handle(new TaoLichHenCommand(ca.IdCaLamViec, dv.IdDichVu, null, null, null, "B"), CancellationToken.None);
        await Task.WhenAll(task1.ContinueWith(_ => { }), task2.ContinueWith(_ => { }));

        var results = new[] { task1, task2 };
        results.Count(t => t.IsCompletedSuccessfully).Should().Be(1);
        results.Count(t => t.IsFaulted).Should().Be(1);
        (await db.LichHen.AsNoTracking().CountAsync()).Should().Be(1);
    }
}
