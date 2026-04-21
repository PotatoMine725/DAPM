using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Features.HoSoKham.Queries.LichSuKhamCuaToi;
using ClinicBooking.Application.UnitTests.Common;
using FluentAssertions;
using NSubstitute;

namespace ClinicBooking.Application.UnitTests.Features.HoSoKham.Queries.LichSuKhamCuaToi;

public sealed class LichSuKhamCuaToiHandlerTests
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
            ChanDoan = "Chan doan",
            NgayKham = now,
            NgayTao = now
        });
        await db.SaveChangesAsync();

        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.IdTaiKhoan.Returns(duLieu.IdTaiKhoanBenhNhan);

        var handler = new LichSuKhamCuaToiHandler(db, currentUser);
        var result = await handler.Handle(new LichSuKhamCuaToiQuery(1, 20), CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].ChanDoan.Should().Be("Chan doan");
    }
}
