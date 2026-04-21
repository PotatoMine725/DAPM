using ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachDichVu;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Entities;
using FluentAssertions;

namespace ClinicBooking.Application.UnitTests.Features.DanhMuc.Queries.DanhSachDichVu;

public sealed class DanhSachDichVuHandlerTests
{
    [Fact]
    public async Task Handle_CoBoLoc_FilterDung()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var chuyenKhoaA = new ChuyenKhoa { TenChuyenKhoa = "Noi", ThoiGianSlotMacDinh = 20, HienThi = true };
        var chuyenKhoaB = new ChuyenKhoa { TenChuyenKhoa = "Ngoai", ThoiGianSlotMacDinh = 20, HienThi = true };
        db.ChuyenKhoa.AddRange(chuyenKhoaA, chuyenKhoaB);
        await db.SaveChangesAsync();

        db.DichVu.AddRange(
            new DichVu
            {
                IdChuyenKhoa = chuyenKhoaA.IdChuyenKhoa,
                TenDichVu = "Kham Noi Tong Quat",
                HienThi = true,
                NgayTao = DateTime.UtcNow
            },
            new DichVu
            {
                IdChuyenKhoa = chuyenKhoaA.IdChuyenKhoa,
                TenDichVu = "Kham Noi Chuyen Sau",
                HienThi = false,
                NgayTao = DateTime.UtcNow
            },
            new DichVu
            {
                IdChuyenKhoa = chuyenKhoaB.IdChuyenKhoa,
                TenDichVu = "Kham Ngoai",
                HienThi = true,
                NgayTao = DateTime.UtcNow
            });
        await db.SaveChangesAsync();

        var handler = new DanhSachDichVuHandler(db);
        var result = await handler.Handle(
            new DanhSachDichVuQuery(1, 20, chuyenKhoaA.IdChuyenKhoa, true, "Noi"),
            CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].TenDichVu.Should().Be("Kham Noi Tong Quat");
        result[0].TenChuyenKhoa.Should().Be("Noi");
    }

    [Fact]
    public async Task Handle_Paging_HoatDongDung()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var chuyenKhoa = new ChuyenKhoa { TenChuyenKhoa = "UT Tai Mui Hong Paging", ThoiGianSlotMacDinh = 20, HienThi = true };
        db.ChuyenKhoa.Add(chuyenKhoa);
        await db.SaveChangesAsync();

        for (var i = 1; i <= 5; i++)
        {
            db.DichVu.Add(new DichVu
            {
                IdChuyenKhoa = chuyenKhoa.IdChuyenKhoa,
                TenDichVu = $"UT DV {i:D2}",
                HienThi = true,
                NgayTao = DateTime.UtcNow
            });
        }

        await db.SaveChangesAsync();

        var handler = new DanhSachDichVuHandler(db);
        var result = await handler.Handle(
            new DanhSachDichVuQuery(2, 2, chuyenKhoa.IdChuyenKhoa, null, "UT DV"),
            CancellationToken.None);

        result.Should().HaveCount(2);
        result[0].TenDichVu.Should().Be("UT DV 03");
    }
}
