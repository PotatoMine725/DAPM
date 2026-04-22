using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Features.Doctors.Queries.DanhSachBacSiCongKhai;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Entities;
using ClinicBooking.Domain.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.UnitTests.Features.Doctors.Queries.DanhSachBacSiCongKhai;

public sealed class DanhSachBacSiCongKhaiHandlerTests
{
    [Fact]
    public async Task Handle_KhongFilter_TraVeDanhSachPublic()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var chuyenKhoa = new ChuyenKhoa
        {
            TenChuyenKhoa = "Noi Tong Quat UT",
            ThoiGianSlotMacDinh = 20,
            HienThi = true
        };
        db.ChuyenKhoa.Add(chuyenKhoa);
        await db.SaveChangesAsync();

        db.BacSi.Add(new BacSi
        {
            IdTaiKhoan = 1,
            IdChuyenKhoa = chuyenKhoa.IdChuyenKhoa,
            HoTen = "Bac Si A",
            LoaiHopDong = LoaiHopDong.ChinhThuc,
            TrangThai = TrangThaiBacSi.DangLam,
            NgayTao = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        var handler = new DanhSachBacSiCongKhaiHandler(db);

        var result = await handler.Handle(new DanhSachBacSiCongKhaiQuery(), CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].HoTen.Should().Be("Bac Si A");
        result[0].TenChuyenKhoa.Should().Be("Noi Tong Quat UT");
    }

    [Fact]
    public async Task Handle_FilterTheoChuyenKhoa_ChiTraVeBacSiThuocKhoa()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var ck1 = new ChuyenKhoa { TenChuyenKhoa = "CK1 UT", ThoiGianSlotMacDinh = 20, HienThi = true };
        var ck2 = new ChuyenKhoa { TenChuyenKhoa = "CK2 UT", ThoiGianSlotMacDinh = 20, HienThi = true };
        db.ChuyenKhoa.AddRange(ck1, ck2);
        await db.SaveChangesAsync();

        db.BacSi.AddRange(
            new BacSi
            {
                IdTaiKhoan = 1,
                IdChuyenKhoa = ck1.IdChuyenKhoa,
                HoTen = "Bac Si A",
                LoaiHopDong = LoaiHopDong.ChinhThuc,
                TrangThai = TrangThaiBacSi.DangLam,
                NgayTao = DateTime.UtcNow
            },
            new BacSi
            {
                IdTaiKhoan = 2,
                IdChuyenKhoa = ck2.IdChuyenKhoa,
                HoTen = "Bac Si B",
                LoaiHopDong = LoaiHopDong.ChinhThuc,
                TrangThai = TrangThaiBacSi.DangLam,
                NgayTao = DateTime.UtcNow
            });
        await db.SaveChangesAsync();

        var handler = new DanhSachBacSiCongKhaiHandler(db);

        var result = await handler.Handle(new DanhSachBacSiCongKhaiQuery(IdChuyenKhoa: ck2.IdChuyenKhoa), CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].HoTen.Should().Be("Bac Si B");
    }
}
