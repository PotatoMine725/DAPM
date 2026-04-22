using ClinicBooking.Application.Features.Doctors.Queries.DanhSachBacSiCongKhai;
using ClinicBooking.Application.UnitTests.Common;
using BacSiEntity = ClinicBooking.Domain.Entities.BacSi;
using ChuyenKhoaEntity = ClinicBooking.Domain.Entities.ChuyenKhoa;
using ClinicBooking.Domain.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.UnitTests.Features.Doctors.Queries.DanhSachBacSiCongKhai;

public sealed class DanhSachBacSiCongKhaiHandlerTests
{
    [Fact]
    public async Task Handle_MacDinh_ChiTraVeBacSiDangLamVaChuyenKhoaHienThi()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var chuyenKhoaHienThi = new ChuyenKhoaEntity { TenChuyenKhoa = "CK-UT-Public-20260422", ThoiGianSlotMacDinh = 20, HienThi = true };
        var chuyenKhoaAn = new ChuyenKhoaEntity { TenChuyenKhoa = "CK-UT-Hidden-20260422", ThoiGianSlotMacDinh = 20, HienThi = false };
        db.ChuyenKhoa.AddRange(chuyenKhoaHienThi, chuyenKhoaAn);
        await db.SaveChangesAsync();

        var tk1 = TestDataSeeder.SeedTaiKhoan(db, VaiTro.BacSi);
        var tk2 = TestDataSeeder.SeedTaiKhoan(db, VaiTro.BacSi);
        var tk3 = TestDataSeeder.SeedTaiKhoan(db, VaiTro.BacSi);

        db.BacSi.AddRange(
            new BacSiEntity
            {
                IdTaiKhoan = tk1.IdTaiKhoan,
                IdChuyenKhoa = chuyenKhoaHienThi.IdChuyenKhoa,
                HoTen = "Bac Si Dang Lam",
                LoaiHopDong = LoaiHopDong.NoiTru,
                TrangThai = TrangThaiBacSi.DangLam,
                NgayTao = DateTime.UtcNow
            },
            new BacSiEntity
            {
                IdTaiKhoan = tk2.IdTaiKhoan,
                IdChuyenKhoa = chuyenKhoaHienThi.IdChuyenKhoa,
                HoTen = "Bac Si Nghi Viec",
                LoaiHopDong = LoaiHopDong.HopDong,
                TrangThai = TrangThaiBacSi.NghiViec,
                NgayTao = DateTime.UtcNow
            },
            new BacSiEntity
            {
                IdTaiKhoan = tk3.IdTaiKhoan,
                IdChuyenKhoa = chuyenKhoaAn.IdChuyenKhoa,
                HoTen = "Bac Si ChuyenKhoa An",
                LoaiHopDong = LoaiHopDong.NoiTru,
                TrangThai = TrangThaiBacSi.DangLam,
                NgayTao = DateTime.UtcNow
            });
        await db.SaveChangesAsync();

        var handler = new DanhSachBacSiCongKhaiHandler(db);

        var result = await handler.Handle(new DanhSachBacSiCongKhaiQuery(), CancellationToken.None);

        result.Should().ContainSingle(x => x.HoTen == "Bac Si Dang Lam");
    }

    [Fact]
    public async Task Handle_FilterTheoChuyenKhoa_ChiTraVeDungNguoi()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var chuyenKhoa1 = new ChuyenKhoaEntity { TenChuyenKhoa = "CK-UT-1", ThoiGianSlotMacDinh = 20, HienThi = true };
        var chuyenKhoa2 = new ChuyenKhoaEntity { TenChuyenKhoa = "CK-UT-2", ThoiGianSlotMacDinh = 20, HienThi = true };
        db.ChuyenKhoa.AddRange(chuyenKhoa1, chuyenKhoa2);
        await db.SaveChangesAsync();

        var tk1 = TestDataSeeder.SeedTaiKhoan(db, VaiTro.BacSi);
        var tk2 = TestDataSeeder.SeedTaiKhoan(db, VaiTro.BacSi);
        db.BacSi.AddRange(
            new BacSiEntity { IdTaiKhoan = tk1.IdTaiKhoan, IdChuyenKhoa = chuyenKhoa1.IdChuyenKhoa, HoTen = "BS1", LoaiHopDong = LoaiHopDong.HopDong, TrangThai = TrangThaiBacSi.DangLam, NgayTao = DateTime.UtcNow },
            new BacSiEntity { IdTaiKhoan = tk2.IdTaiKhoan, IdChuyenKhoa = chuyenKhoa2.IdChuyenKhoa, HoTen = "BS2", LoaiHopDong = LoaiHopDong.NoiTru, TrangThai = TrangThaiBacSi.DangLam, NgayTao = DateTime.UtcNow });
        await db.SaveChangesAsync();

        var handler = new DanhSachBacSiCongKhaiHandler(db);

        var result = await handler.Handle(new DanhSachBacSiCongKhaiQuery(IdChuyenKhoa: chuyenKhoa2.IdChuyenKhoa), CancellationToken.None);

        result.Should().ContainSingle(x => x.HoTen == "BS2");
    }
}
