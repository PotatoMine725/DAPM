using ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachChuyenKhoaCongKhai;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.UnitTests.Features.DanhMuc.Queries.DanhSachChuyenKhoaCongKhai;

public sealed class DanhSachChuyenKhoaCongKhaiHandlerTests
{
    [Fact]
    public async Task Handle_MacDinh_ChiTraVeChuyenKhoaHienThi()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        db.ChuyenKhoa.AddRange(
            new ChuyenKhoa
            {
                TenChuyenKhoa = "CK-UT-Public-20260422",
                ThoiGianSlotMacDinh = 20,
                HienThi = true
            },
            new ChuyenKhoa
            {
                TenChuyenKhoa = "CK-UT-Hidden-20260422",
                ThoiGianSlotMacDinh = 20,
                HienThi = false
            });
        await db.SaveChangesAsync();

        var handler = new DanhSachChuyenKhoaCongKhaiHandler(db);

        var result = await handler.Handle(new DanhSachChuyenKhoaCongKhaiQuery(), CancellationToken.None);

        result.Should().Contain(x => x.TenChuyenKhoa == "CK-UT-Public-20260422");
    }

    [Fact]
    public async Task Handle_TuKhoa_FilterTheoTen()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        db.ChuyenKhoa.AddRange(
            new ChuyenKhoa { TenChuyenKhoa = "ZZZ Tim Mach Public 20260422", ThoiGianSlotMacDinh = 20, HienThi = true },
            new ChuyenKhoa { TenChuyenKhoa = "ZZZ Nhi Khoa Public 20260422", ThoiGianSlotMacDinh = 20, HienThi = true });
        await db.SaveChangesAsync();

        var handler = new DanhSachChuyenKhoaCongKhaiHandler(db);

        var result = await handler.Handle(new DanhSachChuyenKhoaCongKhaiQuery(TuKhoa: "Tim"), CancellationToken.None);

        result.Should().ContainSingle(x => x.TenChuyenKhoa == "ZZZ Tim Mach Public 20260422");
    }
}
