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
    public async Task Handle_MacDinh_ChiTraVeBacSiDangLamVaChuyenKhoaHienThi()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var chuyenKhoaHienThi = new ChuyenKhoa { TenChuyenKhoa = "Noi Tong Quat Public", ThoiGianSlotMacDinh = 20, HienThi = true };
        var chuyenKhoaAn = new ChuyenKhoa { TenChuyenKhoa = "An Public", ThoiGianSlotMacDinh = 20, HienThi = false };
        db.ChuyenKhoa.AddRange(chuyenKhoaHienThi, chuyenKhoaAn);
        await db.SaveChangesAsync();

        db.BacSi.AddRange(
            new BacSi
            {
                IdTaiKhoan = 1,
                IdChuyenKhoa = chuyenKhoaHienThi.IdChuyenKhoa,
                HoTen = "Bac Si Dang Lam",
                LoaiHopDong = LoaiHopDong.ChinhThuc,
                TrangThai = TrangThaiBacSi.DangLam,
                NgayTao = DateTime.UtcNow
            },
            new BacSi
            {
                IdTaiKhoan = 2,
                IdChuyenKhoa = chuyenKhoaHienThi.IdChuyenKhoa,
                HoTen = "Bac Si Nghi Viec",
                LoaiHopDong = LoaiHopDong.ChinhThuc,
                TrangThai = TrangThaiBacSi.NghiViec,
                NgayTao = DateTime.UtcNow
            },
            new BacSi
            {
                IdTaiKhoan = 3,
                IdChuyenKhoa = chuyenKhoaAn.IdChuyenKhoa,
                HoTen = "Bac Si ChuyenKhoa An",
                LoaiHopDong = LoaiHopDong.ChinhThuc,
                TrangThai = TrangThaiBacSi.DangLam,
                NgayTao = DateTime.UtcNow
            });
        await db.SaveChangesAsync();

        var handler = new DanhSachBacSiCongKhaiHandler(db);

        var result = await handler.Handle(new DanhSachBacSiCongKhaiQuery(), CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].HoTen.Should().Be("Bac Si Dang Lam");
        result[0].TenChuyenKhoa.Should().Be("Noi Tong Quat Public");
    }

    [Fact]
    public async Task Handle_FilterTheoChuyenKhoa_ChiTraVeDungNguoi()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var chuyenKhoa1 = new ChuyenKhoa { TenChuyenKhoa = "CK1", ThoiGianSlotMacDinh = 20, HienThi = true };
        var chuyenKhoa2 = new ChuyenKhoa { TenChuyenKhoa = "CK2", ThoiGianSlotMacDinh = 20, HienThi = true };
        db.ChuyenKhoa.AddRange(chuyenKhoa1, chuyenKhoa2);
        await db.SaveChangesAsync();

        db.BacSi.AddRange(
            new BacSi { IdTaiKhoan = 1, IdChuyenKhoa = chuyenKhoa1.IdChuyenKhoa, HoTen = "BS1", LoaiHopDong = LoaiHopDong.ChinhThuc, TrangThai = TrangThaiBacSi.DangLam, NgayTao = DateTime.UtcNow },
            new BacSi { IdTaiKhoan = 2, IdChuyenKhoa = chuyenKhoa2.IdChuyenKhoa, HoTen = "BS2", LoaiHopDong = LoaiHopDong.ChinhThuc, TrangThai = TrangThaiBacSi.DangLam, NgayTao = DateTime.UtcNow });
        await db.SaveChangesAsync();

        var handler = new DanhSachBacSiCongKhaiHandler(db);

        var result = await handler.Handle(new DanhSachBacSiCongKhaiQuery(IdChuyenKhoa: chuyenKhoa2.IdChuyenKhoa), CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].HoTen.Should().Be("BS2");
    }
}
