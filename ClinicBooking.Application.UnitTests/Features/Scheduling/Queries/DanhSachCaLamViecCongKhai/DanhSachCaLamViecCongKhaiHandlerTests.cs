using ClinicBooking.Application.Features.Scheduling.Queries.DanhSachCaLamViecCongKhai;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Entities;
using ClinicBooking.Domain.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.UnitTests.Features.Scheduling.Queries.DanhSachCaLamViecCongKhai;

public sealed class DanhSachCaLamViecCongKhaiHandlerTests
{
    [Fact]
    public async Task Handle_ConTrongTrue_ChiTraVeCaConSlot()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var chuyenKhoa = new ChuyenKhoa { TenChuyenKhoa = "CK UT", ThoiGianSlotMacDinh = 20, HienThi = true };
        var phong = new Phong { MaPhong = "P-UT-001", TenPhong = "Phong UT", SucChua = 1, TrangThai = true };
        var bacSi = new BacSi
        {
            IdTaiKhoan = 1,
            IdChuyenKhoa = 1,
            HoTen = "Bac Si UT",
            LoaiHopDong = LoaiHopDong.ChinhThuc,
            TrangThai = TrangThaiBacSi.DangLam,
            NgayTao = DateTime.UtcNow
        };
        var dinhNghiaCa = new DinhNghiaCa { TenCa = "sang_ut", GioBatDauMacDinh = new TimeOnly(7, 0), GioKetThucMacDinh = new TimeOnly(12, 0), TrangThai = true };
        db.ChuyenKhoa.Add(chuyenKhoa);
        db.Phong.Add(phong);
        db.BacSi.Add(bacSi);
        db.DinhNghiaCa.Add(dinhNghiaCa);
        await db.SaveChangesAsync();

        db.CaLamViec.AddRange(
            new CaLamViec
            {
                IdBacSi = bacSi.IdBacSi,
                IdPhong = phong.IdPhong,
                IdChuyenKhoa = chuyenKhoa.IdChuyenKhoa,
                IdDinhNghiaCa = dinhNghiaCa.IdDinhNghiaCa,
                NgayLamViec = new DateOnly(2026, 4, 23),
                GioBatDau = new TimeOnly(7, 0),
                GioKetThuc = new TimeOnly(12, 0),
                ThoiGianSlot = 20,
                SoSlotToiDa = 10,
                SoSlotDaDat = 3,
                TrangThaiDuyet = TrangThaiDuyetCa.DaDuyet,
                NguonTaoCa = NguonTaoCa.Admin,
                NgayTao = DateTime.UtcNow
            },
            new CaLamViec
            {
                IdBacSi = bacSi.IdBacSi,
                IdPhong = phong.IdPhong,
                IdChuyenKhoa = chuyenKhoa.IdChuyenKhoa,
                IdDinhNghiaCa = dinhNghiaCa.IdDinhNghiaCa,
                NgayLamViec = new DateOnly(2026, 4, 24),
                GioBatDau = new TimeOnly(13, 0),
                GioKetThuc = new TimeOnly(17, 0),
                ThoiGianSlot = 20,
                SoSlotToiDa = 10,
                SoSlotDaDat = 10,
                TrangThaiDuyet = TrangThaiDuyetCa.DaDuyet,
                NguonTaoCa = NguonTaoCa.Admin,
                NgayTao = DateTime.UtcNow
            });
        await db.SaveChangesAsync();

        var handler = new DanhSachCaLamViecCongKhaiHandler(db);

        var result = await handler.Handle(new DanhSachCaLamViecCongKhaiQuery(ConTrong: true), CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].ConTrong.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_FilterTheoBacSi_TraVeDungCa()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var chuyenKhoa = new ChuyenKhoa { TenChuyenKhoa = "CK UT 2", ThoiGianSlotMacDinh = 20, HienThi = true };
        var phong = new Phong { MaPhong = "P-UT-002", TenPhong = "Phong UT 2", SucChua = 1, TrangThai = true };
        var bacSi1 = new BacSi { IdTaiKhoan = 1, IdChuyenKhoa = 1, HoTen = "BS 1", LoaiHopDong = LoaiHopDong.ChinhThuc, TrangThai = TrangThaiBacSi.DangLam, NgayTao = DateTime.UtcNow };
        var bacSi2 = new BacSi { IdTaiKhoan = 2, IdChuyenKhoa = 1, HoTen = "BS 2", LoaiHopDong = LoaiHopDong.ChinhThuc, TrangThai = TrangThaiBacSi.DangLam, NgayTao = DateTime.UtcNow };
        var dinhNghiaCa = new DinhNghiaCa { TenCa = "chieu_ut", GioBatDauMacDinh = new TimeOnly(13, 0), GioKetThucMacDinh = new TimeOnly(17, 0), TrangThai = true };
        db.ChuyenKhoa.Add(chuyenKhoa);
        db.Phong.Add(phong);
        db.BacSi.AddRange(bacSi1, bacSi2);
        db.DinhNghiaCa.Add(dinhNghiaCa);
        await db.SaveChangesAsync();

        db.CaLamViec.AddRange(
            new CaLamViec
            {
                IdBacSi = bacSi1.IdBacSi,
                IdPhong = phong.IdPhong,
                IdChuyenKhoa = chuyenKhoa.IdChuyenKhoa,
                IdDinhNghiaCa = dinhNghiaCa.IdDinhNghiaCa,
                NgayLamViec = new DateOnly(2026, 4, 25),
                GioBatDau = new TimeOnly(13, 0),
                GioKetThuc = new TimeOnly(17, 0),
                ThoiGianSlot = 20,
                SoSlotToiDa = 10,
                SoSlotDaDat = 0,
                TrangThaiDuyet = TrangThaiDuyetCa.DaDuyet,
                NguonTaoCa = NguonTaoCa.Admin,
                NgayTao = DateTime.UtcNow
            },
            new CaLamViec
            {
                IdBacSi = bacSi2.IdBacSi,
                IdPhong = phong.IdPhong,
                IdChuyenKhoa = chuyenKhoa.IdChuyenKhoa,
                IdDinhNghiaCa = dinhNghiaCa.IdDinhNghiaCa,
                NgayLamViec = new DateOnly(2026, 4, 26),
                GioBatDau = new TimeOnly(13, 0),
                GioKetThuc = new TimeOnly(17, 0),
                ThoiGianSlot = 20,
                SoSlotToiDa = 10,
                SoSlotDaDat = 0,
                TrangThaiDuyet = TrangThaiDuyetCa.DaDuyet,
                NguonTaoCa = NguonTaoCa.Admin,
                NgayTao = DateTime.UtcNow
            });
        await db.SaveChangesAsync();

        var handler = new DanhSachCaLamViecCongKhaiHandler(db);

        var result = await handler.Handle(new DanhSachCaLamViecCongKhaiQuery(IdBacSi: bacSi2.IdBacSi), CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].HoTenBacSi.Should().Be("BS 2");
    }
}
