using ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachChuyenKhoa;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Entities;
using FluentAssertions;

namespace ClinicBooking.Application.UnitTests.Features.DanhMuc.Queries.DanhSachChuyenKhoa;

public sealed class DanhSachChuyenKhoaHandlerTests
{
    [Fact]
    public async Task Handle_CoBoLocHienThiVaTuKhoa_FilterDung()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        db.ChuyenKhoa.AddRange(
            new ChuyenKhoa { TenChuyenKhoa = "UT Noi Tong Quat", ThoiGianSlotMacDinh = 20, HienThi = true },
            new ChuyenKhoa { TenChuyenKhoa = "UT Ngoai Tong Quat", ThoiGianSlotMacDinh = 20, HienThi = true },
            new ChuyenKhoa { TenChuyenKhoa = "UT Noi Nhi", ThoiGianSlotMacDinh = 15, HienThi = false });
        await db.SaveChangesAsync();

        var handler = new DanhSachChuyenKhoaHandler(db);
        var result = await handler.Handle(new DanhSachChuyenKhoaQuery(1, 20, true, "UT Noi"), CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].TenChuyenKhoa.Should().Be("UT Noi Tong Quat");
    }

    [Fact]
    public async Task Handle_Paging_HoatDongDung()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        for (var i = 1; i <= 5; i++)
        {
            db.ChuyenKhoa.Add(new ChuyenKhoa
            {
                TenChuyenKhoa = $"UT CK {i:D2}",
                ThoiGianSlotMacDinh = 20,
                HienThi = true
            });
        }

        await db.SaveChangesAsync();

        var handler = new DanhSachChuyenKhoaHandler(db);
        var result = await handler.Handle(new DanhSachChuyenKhoaQuery(2, 2, null, "UT CK"), CancellationToken.None);

        result.Should().HaveCount(2);
        result[0].TenChuyenKhoa.Should().Be("UT CK 03");
    }
}
