using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.LichHen.Commands.GiaiPhongGiuCho;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace ClinicBooking.Application.UnitTests.Features.LichHen.Commands.GiaiPhongGiuCho;

public sealed class GiaiPhongGiuChoHandlerTests
{
    private static readonly DateTime FixedNow = new(2026, 5, 1, 10, 0, 0, DateTimeKind.Utc);

    private static IDateTimeProvider FixedClock()
    {
        var clock = Substitute.For<IDateTimeProvider>();
        clock.UtcNow.Returns(FixedNow);
        return clock;
    }

    [Fact]
    public async Task Handle_GiuChoConHieuLuc_DanhDauDaGiaiPhong()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var ca = TestDataSeeder.SeedCaLamViec(db);
        var bn = TestDataSeeder.SeedBenhNhan(db);
        var giuCho = new GiuCho
        {
            IdCaLamViec = ca.IdCaLamViec,
            IdBenhNhan = bn.IdBenhNhan,
            SoSlot = 1,
            GioHetHan = FixedNow.AddMinutes(15),
            DaGiaiPhong = false,
            NgayTao = FixedNow
        };
        db.GiuCho.Add(giuCho);
        await db.SaveChangesAsync();

        var handler = new GiaiPhongGiuChoHandler(db, FixedClock());

        await handler.Handle(new GiaiPhongGiuChoCommand(giuCho.IdGiuCho), CancellationToken.None);

        var reloaded = await db.GiuCho.AsNoTracking().FirstAsync(x => x.IdGiuCho == giuCho.IdGiuCho);
        reloaded.DaGiaiPhong.Should().BeTrue();
        reloaded.GioHetHan.Should().Be(FixedNow);
    }

    [Fact]
    public async Task Handle_KhongTonTai_ThrowNotFoundException()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var handler = new GiaiPhongGiuChoHandler(db, FixedClock());

        var act = async () => await handler.Handle(new GiaiPhongGiuChoCommand(9999), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Khong tim thay ban ghi giu cho.");
    }

    [Fact]
    public async Task Handle_GiuChoDaGiaiPhong_ThrowConflictException()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var ca = TestDataSeeder.SeedCaLamViec(db);
        var bn = TestDataSeeder.SeedBenhNhan(db);
        var giuCho = new GiuCho
        {
            IdCaLamViec = ca.IdCaLamViec,
            IdBenhNhan = bn.IdBenhNhan,
            SoSlot = 1,
            GioHetHan = FixedNow,
            DaGiaiPhong = true,
            NgayTao = FixedNow
        };
        db.GiuCho.Add(giuCho);
        await db.SaveChangesAsync();

        var handler = new GiaiPhongGiuChoHandler(db, FixedClock());

        var act = async () => await handler.Handle(new GiaiPhongGiuChoCommand(giuCho.IdGiuCho), CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("Ban ghi giu cho da duoc giai phong truoc do.");
    }
}
