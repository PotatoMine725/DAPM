using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Entities;
using ClinicBooking.Domain.Enums;
using ClinicBooking.Infrastructure.Persistence;
using ClinicBooking.Infrastructure.Services.Scheduling;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace ClinicBooking.Application.UnitTests.Features.Scheduling.Services;

public sealed class CaLamViecQueryServiceTests
{
    private static readonly DateTime FixedNow = new(2026, 5, 1, 10, 0, 0, DateTimeKind.Utc);

    private static CaLamViecQueryService CreateService(TestDbContextFactory factory, out AppDbContext db)
    {
        db = factory.CreateContext();
        var clock = Substitute.For<IDateTimeProvider>();
        clock.UtcNow.Returns(FixedNow);
        return new CaLamViecQueryService(db, clock);
    }

    [Fact]
    public async Task LayThongTinCaAsync_KhongTonTai_TraNull()
    {
        using var factory = new TestDbContextFactory();
        var service = CreateService(factory, out _);

        var result = await service.LayThongTinCaAsync(999, CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task LayThongTinCaAsync_CoDuLieu_TraDungThongTin()
    {
        using var factory = new TestDbContextFactory();
        var service = CreateService(factory, out var db);
        SeedCa(db, TrangThaiDuyetCa.DaDuyet, soSlotToiDa: 10, soSlotDaDat: 3, ngay: new DateOnly(2026, 5, 5));

        var result = await service.LayThongTinCaAsync(1, CancellationToken.None);

        result.Should().NotBeNull();
        result!.IdCaLamViec.Should().Be(1);
        result.SoSlotToiDa.Should().Be(10);
        result.SoSlotDaDat.Should().Be(3);
        result.TrangThaiDuyet.Should().Be(TrangThaiDuyetCa.DaDuyet);
    }

    [Fact]
    public async Task LaCaDuocDuyetAsync_CaDaDuyet_TraTrue()
    {
        using var factory = new TestDbContextFactory();
        var service = CreateService(factory, out var db);
        SeedCa(db, TrangThaiDuyetCa.DaDuyet, soSlotToiDa: 10, soSlotDaDat: 0, ngay: new DateOnly(2026, 5, 5));

        var result = await service.LaCaDuocDuyetAsync(1, CancellationToken.None);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task LaCaDuocDuyetAsync_CaChuaDuyet_TraFalse()
    {
        using var factory = new TestDbContextFactory();
        var service = CreateService(factory, out var db);
        SeedCa(db, TrangThaiDuyetCa.ChoDuyet, soSlotToiDa: 10, soSlotDaDat: 0, ngay: new DateOnly(2026, 5, 5));

        var result = await service.LaCaDuocDuyetAsync(1, CancellationToken.None);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task KiemTraSlotTrongAsync_CaKhongTonTai_TraLyDoDung()
    {
        using var factory = new TestDbContextFactory();
        var service = CreateService(factory, out _);

        var result = await service.KiemTraSlotTrongAsync(999, CancellationToken.None);

        result.CoTheDat.Should().BeFalse();
        result.LyDoTuChoi.Should().Be(LyDoKhongDatDuoc.KhongTonTai);
    }

    [Fact]
    public async Task KiemTraSlotTrongAsync_CaChuaDuyet_TraLyDoDung()
    {
        using var factory = new TestDbContextFactory();
        var service = CreateService(factory, out var db);
        SeedCa(db, TrangThaiDuyetCa.ChoDuyet, soSlotToiDa: 10, soSlotDaDat: 0, ngay: new DateOnly(2026, 5, 5));

        var result = await service.KiemTraSlotTrongAsync(1, CancellationToken.None);

        result.CoTheDat.Should().BeFalse();
        result.LyDoTuChoi.Should().Be(LyDoKhongDatDuoc.CaChuaDuyet);
    }

    [Fact]
    public async Task KiemTraSlotTrongAsync_CaDaQuaThoiDiem_TraLyDoDung()
    {
        using var factory = new TestDbContextFactory();
        var service = CreateService(factory, out var db);
        SeedCa(db, TrangThaiDuyetCa.DaDuyet, soSlotToiDa: 10, soSlotDaDat: 0, ngay: new DateOnly(2026, 4, 30));

        var result = await service.KiemTraSlotTrongAsync(1, CancellationToken.None);

        result.CoTheDat.Should().BeFalse();
        result.LyDoTuChoi.Should().Be(LyDoKhongDatDuoc.CaDaDiQua);
    }

    [Fact]
    public async Task KiemTraSlotTrongAsync_HetSlot_BaoHetSlot()
    {
        using var factory = new TestDbContextFactory();
        var service = CreateService(factory, out var db);
        SeedCa(db, TrangThaiDuyetCa.DaDuyet, soSlotToiDa: 2, soSlotDaDat: 2, ngay: new DateOnly(2026, 5, 5));

        var result = await service.KiemTraSlotTrongAsync(1, CancellationToken.None);

        result.CoTheDat.Should().BeFalse();
        result.LyDoTuChoi.Should().Be(LyDoKhongDatDuoc.HetSlot);
    }

    [Fact]
    public async Task KiemTraSlotTrongAsync_CoGiuChoHieuLuc_TinhVaoSoSlotDaDung()
    {
        using var factory = new TestDbContextFactory();
        var service = CreateService(factory, out var db);
        SeedCa(db, TrangThaiDuyetCa.DaDuyet, soSlotToiDa: 3, soSlotDaDat: 1, ngay: new DateOnly(2026, 5, 5));
        db.GiuCho.Add(new GiuCho
        {
            IdCaLamViec = 1,
            IdBenhNhan = 1,
            SoSlot = 2,
            GioHetHan = FixedNow.AddMinutes(15),
            DaGiaiPhong = false,
            NgayTao = FixedNow
        });
        await db.SaveChangesAsync();

        var result = await service.KiemTraSlotTrongAsync(1, CancellationToken.None);

        result.CoTheDat.Should().BeFalse();
        result.SoGiuChoHieuLuc.Should().Be(1);
        result.LyDoTuChoi.Should().Be(LyDoKhongDatDuoc.HetSlot);
    }

    [Fact]
    public async Task IncrementSoSlotDaDatAsync_TangVaGiam_AnToanTheoRangBuoc()
    {
        using var factory = new TestDbContextFactory();
        var service = CreateService(factory, out var db);
        SeedCa(db, TrangThaiDuyetCa.DaDuyet, soSlotToiDa: 5, soSlotDaDat: 1, ngay: new DateOnly(2026, 5, 5));

        var tang = await service.IncrementSoSlotDaDatAsync(1, 1, CancellationToken.None);
        tang.Should().Be(2);

        var giam = await service.IncrementSoSlotDaDatAsync(1, -1, CancellationToken.None);
        giam.Should().Be(1);

        var vuotTran = await service.IncrementSoSlotDaDatAsync(1, 10, CancellationToken.None);
        vuotTran.Should().BeNull();
    }

    [Fact]
    public async Task IncrementSoSlotDaDatAsync_KhongDuocAm_ThiTraNull()
    {
        using var factory = new TestDbContextFactory();
        var service = CreateService(factory, out var db);
        SeedCa(db, TrangThaiDuyetCa.DaDuyet, soSlotToiDa: 5, soSlotDaDat: 0, ngay: new DateOnly(2026, 5, 5));

        var result = await service.IncrementSoSlotDaDatAsync(1, -1, CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task IncrementSoSlotDaDatAsync_CaKhongTonTai_TraNull()
    {
        using var factory = new TestDbContextFactory();
        var service = CreateService(factory, out _);

        var result = await service.IncrementSoSlotDaDatAsync(999, 1, CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task IncrementSoSlotDaDatAsync_HaiRequestDongThoi_ChiMotRequestThanhCong()
    {
        using var factory = new TestDbContextFactory();
        using var dbSetup = factory.CreateContext();
        SeedCa(dbSetup, TrangThaiDuyetCa.DaDuyet, soSlotToiDa: 1, soSlotDaDat: 0, ngay: new DateOnly(2026, 5, 5));

        using var db1 = factory.CreateContext();
        using var db2 = factory.CreateContext();
        var clock1 = Substitute.For<IDateTimeProvider>();
        clock1.UtcNow.Returns(FixedNow);
        var clock2 = Substitute.For<IDateTimeProvider>();
        clock2.UtcNow.Returns(FixedNow);
        var service1 = new CaLamViecQueryService(db1, clock1);
        var service2 = new CaLamViecQueryService(db2, clock2);

        var task1 = service1.IncrementSoSlotDaDatAsync(1, 1, CancellationToken.None);
        var task2 = service2.IncrementSoSlotDaDatAsync(1, 1, CancellationToken.None);
        await Task.WhenAll(task1.ContinueWith(_ => { }), task2.ContinueWith(_ => { }));

        var ketQua1 = await task1;
        var ketQua2 = await task2;

        new[] { ketQua1, ketQua2 }.Should().ContainSingle(x => x is not null);
        new[] { ketQua1, ketQua2 }.Should().ContainSingle(x => x is null);

        using var dbCheck = factory.CreateContext();
        (await dbCheck.CaLamViec.AsNoTracking().SingleAsync(x => x.IdCaLamViec == 1)).SoSlotDaDat.Should().Be(1);
    }

    private static void SeedCa(AppDbContext db, TrangThaiDuyetCa trangThai, int soSlotToiDa, int soSlotDaDat, DateOnly ngay)
    {
        db.CaLamViec.Add(new CaLamViec
        {
            IdBacSi = 1,
            IdPhong = 1,
            IdChuyenKhoa = 1,
            IdDinhNghiaCa = 1,
            NgayLamViec = ngay,
            GioBatDau = new TimeOnly(8, 0),
            GioKetThuc = new TimeOnly(11, 0),
            ThoiGianSlot = 15,
            SoSlotToiDa = soSlotToiDa,
            SoSlotDaDat = soSlotDaDat,
            TrangThaiDuyet = trangThai,
            NguonTaoCa = NguonTaoCa.TuDong,
            NgayTao = FixedNow
        });

        db.BacSi.Add(new BacSi
        {
            IdBacSi = 1,
            IdTaiKhoan = 1,
            IdChuyenKhoa = 1,
            HoTen = "BS UT",
            LoaiHopDong = LoaiHopDong.NoiTru,
            TrangThai = TrangThaiBacSi.DangLam,
            NgayTao = FixedNow
        });

        db.ChuyenKhoa.Add(new ChuyenKhoa { IdChuyenKhoa = 1, TenChuyenKhoa = "CK UT", ThoiGianSlotMacDinh = 15, HienThi = true });
        db.Phong.Add(new Phong { IdPhong = 1, MaPhong = "P-UT-1", TenPhong = "Phong UT", SucChua = 1, TrangThai = true });
        db.DinhNghiaCa.Add(new DinhNghiaCa { IdDinhNghiaCa = 1, TenCa = "Sang", GioBatDauMacDinh = new TimeOnly(8, 0), GioKetThucMacDinh = new TimeOnly(11, 0), TrangThai = true });

        db.TaiKhoan.Add(new TaiKhoan
        {
            IdTaiKhoan = 1,
            TenDangNhap = "user-ut",
            Email = "user-ut@example.com",
            SoDienThoai = "0900000001",
            HoTen = "User UT",
            MatKhauHash = "hash",
            VaiTro = VaiTro.BacSi,
            TrangThai = true,
            NgayTao = FixedNow
        });

        db.SaveChanges();
    }
}
