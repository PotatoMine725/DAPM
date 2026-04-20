using ClinicBooking.Application.Features.Thuoc.Queries.DanhSachThuoc;
using ClinicBooking.Application.UnitTests.Common;
using FluentAssertions;

namespace ClinicBooking.Application.UnitTests.Features.Thuoc.Queries.DanhSachThuoc;

public sealed class DanhSachThuocHandlerTests
{
    [Fact]
    public async Task Handle_CoTuKhoa_FilterDung()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        db.Thuoc.Add(new ClinicBooking.Domain.Entities.Thuoc { TenThuoc = "Paracetamol 500mg", HoatChat = "Paracetamol" });
        db.Thuoc.Add(new ClinicBooking.Domain.Entities.Thuoc { TenThuoc = "Amoxicillin 500mg", HoatChat = "Amoxicillin" });
        await db.SaveChangesAsync();

        var handler = new DanhSachThuocHandler(db);
        var result = await handler.Handle(new DanhSachThuocQuery(1, 20, "Paracetamol"), CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].TenThuoc.Should().Be("Paracetamol 500mg");
    }

    [Fact]
    public async Task Handle_Paging_HoatDongDung()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        for (var i = 1; i <= 5; i++)
        {
            db.Thuoc.Add(new ClinicBooking.Domain.Entities.Thuoc { TenThuoc = $"Thuoc {i:D2}" });
        }

        await db.SaveChangesAsync();

        var handler = new DanhSachThuocHandler(db);
        var result = await handler.Handle(new DanhSachThuocQuery(2, 2, null), CancellationToken.None);

        result.Should().HaveCount(2);
    }
}
