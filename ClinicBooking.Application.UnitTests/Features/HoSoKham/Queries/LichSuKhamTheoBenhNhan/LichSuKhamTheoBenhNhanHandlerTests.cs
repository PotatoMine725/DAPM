using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.HoSoKham.Queries.LichSuKhamTheoBenhNhan;
using ClinicBooking.Application.UnitTests.Common;
using FluentAssertions;

namespace ClinicBooking.Application.UnitTests.Features.HoSoKham.Queries.LichSuKhamTheoBenhNhan;

public sealed class LichSuKhamTheoBenhNhanHandlerTests
{
    [Fact]
    public async Task Handle_HopLe_TraVeDanhSach()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var now = DateTime.UtcNow;
        var duLieu = await HoSoKhamTestDataSeeder.TaoAsync(db, now);

        db.HoSoKham.Add(new ClinicBooking.Domain.Entities.HoSoKham
        {
            IdLichHen = duLieu.IdLichHen,
            IdBacSi = duLieu.IdBacSi,
            ChanDoan = "A",
            NgayKham = now,
            NgayTao = now
        });
        await db.SaveChangesAsync();

        var handler = new LichSuKhamTheoBenhNhanHandler(db);
        var result = await handler.Handle(
            new LichSuKhamTheoBenhNhanQuery(duLieu.IdBenhNhan, 1, 20),
            CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].ChanDoan.Should().Be("A");
    }

    [Fact]
    public async Task Handle_BenhNhanKhongTonTai_ThrowNotFoundException()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var handler = new LichSuKhamTheoBenhNhanHandler(db);

        var act = async () => await handler.Handle(
            new LichSuKhamTheoBenhNhanQuery(9999, 1, 20),
            CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Khong tim thay benh nhan.");
    }
}
