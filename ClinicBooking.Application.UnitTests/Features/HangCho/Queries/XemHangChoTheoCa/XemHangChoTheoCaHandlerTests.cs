using ClinicBooking.Application.Features.HangCho.Queries.XemHangChoTheoCa;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Entities;
using ClinicBooking.Domain.Enums;
using FluentAssertions;

namespace ClinicBooking.Application.UnitTests.Features.HangCho.Queries.XemHangChoTheoCa;

public sealed class XemHangChoTheoCaHandlerTests
{
    [Fact]
    public async Task Handle_SapXepTheoSoThuTu()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var bn = TestDataSeeder.SeedBenhNhan(db);
        var ca = TestDataSeeder.SeedCaLamViec(db);
        var lh1 = TestDataSeeder.SeedLichHen(db, bn.IdBenhNhan, ca.IdCaLamViec, soSlot: 1);
        var lh2 = TestDataSeeder.SeedLichHen(db, bn.IdBenhNhan, ca.IdCaLamViec, soSlot: 2);
        db.HangCho.AddRange(
            new ClinicBooking.Domain.Entities.HangCho { IdCaLamViec = ca.IdCaLamViec, IdLichHen = lh2.IdLichHen, SoThuTu = 2, TrangThai = TrangThaiHangCho.ChoKham, NgayCheckIn = DateTime.UtcNow },
            new ClinicBooking.Domain.Entities.HangCho { IdCaLamViec = ca.IdCaLamViec, IdLichHen = lh1.IdLichHen, SoThuTu = 1, TrangThai = TrangThaiHangCho.ChoKham, NgayCheckIn = DateTime.UtcNow });
        await db.SaveChangesAsync();

        var handler = new XemHangChoTheoCaHandler(db);
        var result = await handler.Handle(new XemHangChoTheoCaQuery(ca.IdCaLamViec), CancellationToken.None);

        result.Should().HaveCount(2);
        result.Select(x => x.SoThuTu).Should().ContainInOrder(1, 2);
    }

    [Fact]
    public async Task Handle_KhongCoHangCho_TraVeRong()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var handler = new XemHangChoTheoCaHandler(db);
        var result = await handler.Handle(new XemHangChoTheoCaQuery(999), CancellationToken.None);

        result.Should().BeEmpty();
    }
}
