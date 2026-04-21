using ClinicBooking.Application.Features.LichHen.Queries.DanhSachLichHenTheoNgay;
using ClinicBooking.Application.UnitTests.Common;
using FluentAssertions;

namespace ClinicBooking.Application.UnitTests.Features.LichHen.Queries.DanhSachLichHenTheoNgay;

public sealed class DanhSachLichHenTheoNgayHandlerTests
{
    [Fact]
    public async Task Handle_TraVeChiLichHenTrongNgay()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var bn = TestDataSeeder.SeedBenhNhan(db);
        var caNgayA = TestDataSeeder.SeedCaLamViec(db, new DateOnly(2026, 5, 5));
        var caNgayB = TestDataSeeder.SeedCaLamViec(db, new DateOnly(2026, 5, 6));
        TestDataSeeder.SeedLichHen(db, bn.IdBenhNhan, caNgayA.IdCaLamViec, soSlot: 1);
        TestDataSeeder.SeedLichHen(db, bn.IdBenhNhan, caNgayA.IdCaLamViec, soSlot: 2);
        TestDataSeeder.SeedLichHen(db, bn.IdBenhNhan, caNgayB.IdCaLamViec, soSlot: 1);

        var handler = new DanhSachLichHenTheoNgayHandler(db);
        var result = await handler.Handle(
            new DanhSachLichHenTheoNgayQuery(new DateOnly(2026, 5, 5)), CancellationToken.None);

        result.Should().HaveCount(2);
        result.Should().OnlyContain(x => x.NgayLamViec == new DateOnly(2026, 5, 5));
    }

    [Fact]
    public async Task Handle_KhongCoLichHen_TraVeRong()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var handler = new DanhSachLichHenTheoNgayHandler(db);
        var result = await handler.Handle(
            new DanhSachLichHenTheoNgayQuery(new DateOnly(2026, 5, 5)), CancellationToken.None);

        result.Should().BeEmpty();
    }
}
