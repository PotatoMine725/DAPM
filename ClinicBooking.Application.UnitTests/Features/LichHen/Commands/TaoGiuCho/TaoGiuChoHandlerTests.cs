using ClinicBooking.Application.Abstractions.Scheduling;
using ClinicBooking.Application.Abstractions.Scheduling.Dtos;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Common.Options;
using ClinicBooking.Application.Features.LichHen.Commands.TaoGiuCho;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace ClinicBooking.Application.UnitTests.Features.LichHen.Commands.TaoGiuCho;

public sealed class TaoGiuChoHandlerTests
{
    private static readonly DateTime FixedNow = new(2026, 5, 1, 10, 0, 0, DateTimeKind.Utc);

    private static IOptions<LichHenOptions> DefaultOptions() =>
        Options.Create(new LichHenOptions { HuyMuonTruocGio = 24, GiuChoThoiHanPhut = 15, MaLichHenPrefix = "LH" });

    private static (ICurrentUserService user, IDateTimeProvider clock, ICaLamViecQueryService scheduling) CreateDeps(VaiTro vaiTro)
    {
        var user = Substitute.For<ICurrentUserService>();
        user.VaiTro.Returns(vaiTro);
        user.IdTaiKhoan.Returns(1);
        var clock = Substitute.For<IDateTimeProvider>();
        clock.UtcNow.Returns(FixedNow);
        var scheduling = Substitute.For<ICaLamViecQueryService>();
        return (user, clock, scheduling);
    }

    private static ThongTinCaLamViecDto ThongTinOk(int idCa) => new(
        idCa, 1, 1, 1, 1,
        new DateOnly(2026, 5, 5), new TimeOnly(8, 0), new TimeOnly(11, 0),
        15, 10, 0, TrangThaiDuyetCa.DaDuyet);

    [Fact]
    public async Task Handle_LeTanTaoGiuCho_Ok_TraSoSlotTuStub()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var bn = TestDataSeeder.SeedBenhNhan(db);
        var ca = TestDataSeeder.SeedCaLamViec(db);
        var (user, clock, scheduling) = CreateDeps(VaiTro.LeTan);
        scheduling.LayThongTinCaAsync(ca.IdCaLamViec, Arg.Any<CancellationToken>()).Returns(ThongTinOk(ca.IdCaLamViec));
        scheduling.KiemTraSlotTrongAsync(ca.IdCaLamViec, Arg.Any<CancellationToken>())
            .Returns(new KetQuaKiemTraSlotDto(true, 10, 0, 0, null));
        scheduling.IncrementSoSlotDaDatAsync(ca.IdCaLamViec, 1, Arg.Any<CancellationToken>()).Returns((int?)1);

        var handler = new TaoGiuChoHandler(db, user, clock, scheduling, DefaultOptions());
        var result = await handler.Handle(
            new TaoGiuChoCommand(ca.IdCaLamViec, bn.IdBenhNhan), CancellationToken.None);

        result.SoSlot.Should().Be(1);
        result.GioHetHan.Should().Be(FixedNow.AddMinutes(15));
        (await db.GiuCho.AsNoTracking().CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task Handle_BenhNhanTao_ThrowForbidden()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var bn = TestDataSeeder.SeedBenhNhan(db);
        var ca = TestDataSeeder.SeedCaLamViec(db);
        var (user, clock, scheduling) = CreateDeps(VaiTro.BenhNhan);

        var handler = new TaoGiuChoHandler(db, user, clock, scheduling, DefaultOptions());
        var act = async () => await handler.Handle(
            new TaoGiuChoCommand(ca.IdCaLamViec, bn.IdBenhNhan), CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task Handle_HetSlot_IncrementNull_ThrowConflict()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var bn = TestDataSeeder.SeedBenhNhan(db);
        var ca = TestDataSeeder.SeedCaLamViec(db);
        var (user, clock, scheduling) = CreateDeps(VaiTro.LeTan);
        scheduling.LayThongTinCaAsync(ca.IdCaLamViec, Arg.Any<CancellationToken>()).Returns(ThongTinOk(ca.IdCaLamViec));
        scheduling.KiemTraSlotTrongAsync(ca.IdCaLamViec, Arg.Any<CancellationToken>())
            .Returns(new KetQuaKiemTraSlotDto(true, 10, 10, 0, null));
        scheduling.IncrementSoSlotDaDatAsync(ca.IdCaLamViec, 1, Arg.Any<CancellationToken>()).Returns((int?)null);

        var handler = new TaoGiuChoHandler(db, user, clock, scheduling, DefaultOptions());
        var act = async () => await handler.Handle(
            new TaoGiuChoCommand(ca.IdCaLamViec, bn.IdBenhNhan), CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>().WithMessage("Ca lam viec da het slot.");
    }

    [Fact]
    public async Task Handle_HaiGiuChoDongThoi_ChiMotNguoiDatDuoc()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var tk1 = TestDataSeeder.SeedTaiKhoan(db, VaiTro.BenhNhan);
        var bn1 = TestDataSeeder.SeedBenhNhan(db, idTaiKhoan: tk1.IdTaiKhoan);
        var tk2 = TestDataSeeder.SeedTaiKhoan(db, VaiTro.BenhNhan);
        var bn2 = TestDataSeeder.SeedBenhNhan(db, idTaiKhoan: tk2.IdTaiKhoan);
        var ca = TestDataSeeder.SeedCaLamViec(db);
        var thongTin = ThongTinOk(ca.IdCaLamViec);

        var (u1, c1, s1) = CreateDeps(VaiTro.LeTan);
        var (u2, c2, s2) = CreateDeps(VaiTro.LeTan);
        s1.LayThongTinCaAsync(ca.IdCaLamViec, Arg.Any<CancellationToken>()).Returns(thongTin);
        s2.LayThongTinCaAsync(ca.IdCaLamViec, Arg.Any<CancellationToken>()).Returns(thongTin);
        s1.KiemTraSlotTrongAsync(ca.IdCaLamViec, Arg.Any<CancellationToken>()).Returns(new KetQuaKiemTraSlotDto(true, 10, 0, 0, null));
        s2.KiemTraSlotTrongAsync(ca.IdCaLamViec, Arg.Any<CancellationToken>()).Returns(new KetQuaKiemTraSlotDto(true, 10, 0, 0, null));
        s1.IncrementSoSlotDaDatAsync(ca.IdCaLamViec, 1, Arg.Any<CancellationToken>()).Returns((int?)1);
        s2.IncrementSoSlotDaDatAsync(ca.IdCaLamViec, 1, Arg.Any<CancellationToken>()).Returns((int?)null);

        var h1 = new TaoGiuChoHandler(db, u1, c1, s1, DefaultOptions());
        var h2 = new TaoGiuChoHandler(db, u2, c2, s2, DefaultOptions());

        var t1 = h1.Handle(new TaoGiuChoCommand(ca.IdCaLamViec, bn1.IdBenhNhan), CancellationToken.None);
        var t2 = h2.Handle(new TaoGiuChoCommand(ca.IdCaLamViec, bn2.IdBenhNhan), CancellationToken.None);
        await Task.WhenAll(t1.ContinueWith(_ => { }), t2.ContinueWith(_ => { }));

        var results = new[] { t1, t2 };
        results.Count(t => t.IsCompletedSuccessfully).Should().Be(1);
        results.Count(t => t.IsFaulted).Should().Be(1);
        (await db.GiuCho.AsNoTracking().CountAsync()).Should().Be(1);
    }
}
