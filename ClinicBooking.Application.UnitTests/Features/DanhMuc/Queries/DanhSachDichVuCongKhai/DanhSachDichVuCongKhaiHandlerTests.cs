using ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachDichVuCongKhai;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.UnitTests.Features.DanhMuc.Queries.DanhSachDichVuCongKhai;

public sealed class DanhSachDichVuCongKhaiHandlerTests
{
    [Fact]
    public async Task Handle_MacDinh_ChiTraVeDichVuHienThiVaChuyenKhoaHienThi()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var ckHienThi = new ChuyenKhoa
        {
            TenChuyenKhoa = "ZZZ CK Public 20260422",
            ThoiGianSlotMacDinh = 20,
            HienThi = true
        };
        var ckAn = new ChuyenKhoa
        {
            TenChuyenKhoa = "ZZZ CK Private 20260422",
            ThoiGianSlotMacDinh = 20,
            HienThi = false
        };
        db.ChuyenKhoa.AddRange(ckHienThi, ckAn);
        await db.SaveChangesAsync();

        db.DichVu.AddRange(
            new DichVu
            {
                IdChuyenKhoa = ckHienThi.IdChuyenKhoa,
                TenDichVu = "ZZZ DV Public 20260422",
                HienThi = true,
                NgayTao = DateTime.UtcNow
            },
            new DichVu
            {
                IdChuyenKhoa = ckHienThi.IdChuyenKhoa,
                TenDichVu = "ZZZ DV Hidden 20260422",
                HienThi = false,
                NgayTao = DateTime.UtcNow
            },
            new DichVu
            {
                IdChuyenKhoa = ckAn.IdChuyenKhoa,
                TenDichVu = "ZZZ DV Ck An 20260422",
                HienThi = true,
                NgayTao = DateTime.UtcNow
            });
        await db.SaveChangesAsync();

        var handler = new DanhSachDichVuCongKhaiHandler(db);

        var result = await handler.Handle(new DanhSachDichVuCongKhaiQuery(), CancellationToken.None);

        result.Should().ContainSingle(x => x.TenDichVu == "ZZZ DV Public 20260422" && x.TenChuyenKhoa == "ZZZ CK Public 20260422");
    }

    [Fact]
    public async Task Handle_IdChuyenKhoa_FilterDungDichVu()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var ck1 = new ChuyenKhoa { TenChuyenKhoa = "CK 1", ThoiGianSlotMacDinh = 20, HienThi = true };
        var ck2 = new ChuyenKhoa { TenChuyenKhoa = "CK 2", ThoiGianSlotMacDinh = 20, HienThi = true };
        db.ChuyenKhoa.AddRange(ck1, ck2);
        await db.SaveChangesAsync();

        db.DichVu.AddRange(
            new DichVu { IdChuyenKhoa = ck1.IdChuyenKhoa, TenDichVu = "ZZZ DV 1 20260422", HienThi = true, NgayTao = DateTime.UtcNow },
            new DichVu { IdChuyenKhoa = ck2.IdChuyenKhoa, TenDichVu = "ZZZ DV 2 20260422", HienThi = true, NgayTao = DateTime.UtcNow });
        await db.SaveChangesAsync();

        var handler = new DanhSachDichVuCongKhaiHandler(db);

        var result = await handler.Handle(new DanhSachDichVuCongKhaiQuery(IdChuyenKhoa: ck2.IdChuyenKhoa), CancellationToken.None);

        result.Should().ContainSingle(x => x.TenDichVu == "ZZZ DV 2 20260422");
    }
}
