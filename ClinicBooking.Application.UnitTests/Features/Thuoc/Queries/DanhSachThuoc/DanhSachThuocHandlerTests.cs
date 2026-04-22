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

        db.Thuoc.Add(new ClinicBooking.Domain.Entities.Thuoc { TenThuoc = "THUOC-UT-Paracetamol-500mg-20260422", HoatChat = "Paracetamol" });
        db.Thuoc.Add(new ClinicBooking.Domain.Entities.Thuoc { TenThuoc = "THUOC-UT-Amoxicillin-500mg-20260422", HoatChat = "Amoxicillin" });
        await db.SaveChangesAsync();

        var handler = new DanhSachThuocHandler(db);
        var result = await handler.Handle(new DanhSachThuocQuery(1, 20, "Paracetamol"), CancellationToken.None);

        result.Should().ContainSingle(x => x.TenThuoc == "THUOC-UT-Paracetamol-500mg-20260422");
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
