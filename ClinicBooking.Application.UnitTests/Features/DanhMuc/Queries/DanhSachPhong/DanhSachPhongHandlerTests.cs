using ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachPhong;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Entities;
using FluentAssertions;

namespace ClinicBooking.Application.UnitTests.Features.DanhMuc.Queries.DanhSachPhong;

public sealed class DanhSachPhongHandlerTests
{
    [Fact]
    public async Task Handle_CoBoLocTrangThaiVaTuKhoa_FilterDung()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        db.Phong.AddRange(
            new Phong { MaPhong = "P01", TenPhong = "Phong Noi", TrangThai = true },
            new Phong { MaPhong = "P02", TenPhong = "Phong Ngoai", TrangThai = true },
            new Phong { MaPhong = "P03", TenPhong = "Phong Noi Cu", TrangThai = false });
        await db.SaveChangesAsync();

        var handler = new DanhSachPhongHandler(db);
        var result = await handler.Handle(new DanhSachPhongQuery(1, 20, true, "P01"), CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].MaPhong.Should().Be("P01");
    }

    [Fact]
    public async Task Handle_Paging_HoatDongDung()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        for (var i = 1; i <= 5; i++)
        {
            db.Phong.Add(new Phong
            {
                MaPhong = $"P{i:D2}",
                TenPhong = $"Phong {i:D2}",
                TrangThai = true
            });
        }

        await db.SaveChangesAsync();

        var handler = new DanhSachPhongHandler(db);
        var result = await handler.Handle(new DanhSachPhongQuery(2, 2, null, null), CancellationToken.None);

        result.Should().HaveCount(2);
        result[0].MaPhong.Should().Be("P03");
    }
}
