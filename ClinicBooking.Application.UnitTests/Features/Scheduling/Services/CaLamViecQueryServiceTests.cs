using ClinicBooking.Application.Abstractions.Scheduling.Dtos;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Entities;
using ClinicBooking.Domain.Enums;
using EntityLichHen = ClinicBooking.Domain.Entities.LichHen;
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
        var logger = Substitute.For<Microsoft.Extensions.Logging.ILogger<CaLamViecQueryService>>();
        return new CaLamViecQueryService(db, clock, logger);
    }

    private static CaLamViec SeedCa(AppDbContext db, TrangThaiDuyetCa trangThai, int soSlotToiDa, int soSlotDaDat, DateOnly ngay)
    {
        var chuyenKhoa = TestDataSeeder.SeedChuyenKhoa(db, $"CK UT {Guid.NewGuid():N}");
        var taiKhoan = TestDataSeeder.SeedTaiKhoan(db, VaiTro.BacSi, email: $"bs-{Guid.NewGuid():N}@ut.vn");
        var phong = TestDataSeeder.SeedPhong(db);
        var dinhNghiaCa = TestDataSeeder.SeedDinhNghiaCa(db);

        var bacSi = new ClinicBooking.Domain.Entities.BacSi
        {
            IdTaiKhoan = taiKhoan.IdTaiKhoan,
            IdChuyenKhoa = chuyenKhoa.IdChuyenKhoa,
            HoTen = "BS UT",
            LoaiHopDong = LoaiHopDong.NoiTru,
            TrangThai = TrangThaiBacSi.DangLam,
            NgayTao = FixedNow
        };
        db.BacSi.Add(bacSi);
        db.SaveChanges();

        var ca = new CaLamViec
        {
            IdBacSi = bacSi.IdBacSi,
            IdPhong = phong.IdPhong,
            IdChuyenKhoa = chuyenKhoa.IdChuyenKhoa,
            IdDinhNghiaCa = dinhNghiaCa.IdDinhNghiaCa,
            NgayLamViec = ngay,
            GioBatDau = new TimeOnly(8, 0),
            GioKetThuc = new TimeOnly(11, 0),
            ThoiGianSlot = 15,
            SoSlotToiDa = soSlotToiDa,
            SoSlotDaDat = soSlotDaDat,
            TrangThaiDuyet = trangThai,
            NguonTaoCa = NguonTaoCa.TuDong,
            NgayTao = FixedNow
        };
        db.CaLamViec.Add(ca);
        db.SaveChanges();
        return ca;
    }

    [Fact]
    public async Task LayThongTinCaAsync_KhongTonTai_TraNull() { using var factory = new TestDbContextFactory(); var service = CreateService(factory, out _); (await service.LayThongTinCaAsync(999, CancellationToken.None)).Should().BeNull(); }
    [Fact]
    public async Task LayThongTinCaAsync_CoDuLieu_TraDungThongTin() { using var factory = new TestDbContextFactory(); var service = CreateService(factory, out var db); var ca = SeedCa(db, TrangThaiDuyetCa.DaDuyet, 10, 3, new DateOnly(2026, 5, 5)); var result = await service.LayThongTinCaAsync(ca.IdCaLamViec, CancellationToken.None); result.Should().NotBeNull(); result!.IdCaLamViec.Should().Be(ca.IdCaLamViec); result.SoSlotToiDa.Should().Be(10); result.SoSlotDaDat.Should().Be(3); result.TrangThaiDuyet.Should().Be(TrangThaiDuyetCa.DaDuyet); }
    [Fact]
    public async Task LaCaDuocDuyetAsync_CaDaDuyet_TraTrue() { using var factory = new TestDbContextFactory(); var service = CreateService(factory, out var db); var ca = SeedCa(db, TrangThaiDuyetCa.DaDuyet, 10, 0, new DateOnly(2026, 5, 5)); (await service.LaCaDuocDuyetAsync(ca.IdCaLamViec, CancellationToken.None)).Should().BeTrue(); }
    [Fact]
    public async Task LaCaDuocDuyetAsync_CaChuaDuyet_TraFalse() { using var factory = new TestDbContextFactory(); var service = CreateService(factory, out var db); var ca = SeedCa(db, TrangThaiDuyetCa.ChoDuyet, 10, 0, new DateOnly(2026, 5, 5)); (await service.LaCaDuocDuyetAsync(ca.IdCaLamViec, CancellationToken.None)).Should().BeFalse(); }
    [Fact]
    public async Task KiemTraSlotTrongAsync_CaKhongTonTai_TraLyDoDung() { using var factory = new TestDbContextFactory(); var service = CreateService(factory, out _); var result = await service.KiemTraSlotTrongAsync(999, CancellationToken.None); result.CoTheDat.Should().BeFalse(); result.LyDoTuChoi.Should().Be(LyDoKhongDatDuoc.KhongTonTai); }
    [Fact]
    public async Task KiemTraSlotTrongAsync_CaChuaDuyet_TraLyDoDung() { using var factory = new TestDbContextFactory(); var service = CreateService(factory, out var db); var ca = SeedCa(db, TrangThaiDuyetCa.ChoDuyet, 10, 0, new DateOnly(2026, 5, 5)); var result = await service.KiemTraSlotTrongAsync(ca.IdCaLamViec, CancellationToken.None); result.CoTheDat.Should().BeFalse(); result.LyDoTuChoi.Should().Be(LyDoKhongDatDuoc.CaChuaDuyet); }
    [Fact]
    public async Task KiemTraSlotTrongAsync_CaDaQuaThoiDiem_TraLyDoDung() { using var factory = new TestDbContextFactory(); var service = CreateService(factory, out var db); var ca = SeedCa(db, TrangThaiDuyetCa.DaDuyet, 10, 0, new DateOnly(2026, 4, 30)); var result = await service.KiemTraSlotTrongAsync(ca.IdCaLamViec, CancellationToken.None); result.CoTheDat.Should().BeFalse(); result.LyDoTuChoi.Should().Be(LyDoKhongDatDuoc.CaDaDiQua); }
    [Fact]
    public async Task KiemTraSlotTrongAsync_HetSlot_BaoHetSlot() { using var factory = new TestDbContextFactory(); var service = CreateService(factory, out var db); var ca = SeedCa(db, TrangThaiDuyetCa.DaDuyet, 2, 2, new DateOnly(2026, 5, 5)); var result = await service.KiemTraSlotTrongAsync(ca.IdCaLamViec, CancellationToken.None); result.CoTheDat.Should().BeFalse(); result.LyDoTuChoi.Should().Be(LyDoKhongDatDuoc.HetSlot); }
    [Fact]
    public async Task KiemTraSlotTrongAsync_CoGiuChoHieuLuc_TinhVaoSoSlotDaDung() { using var factory = new TestDbContextFactory(); var service = CreateService(factory, out var db); var ca = SeedCa(db, TrangThaiDuyetCa.DaDuyet, 2, 1, new DateOnly(2026, 5, 5)); var benhNhan = TestDataSeeder.SeedBenhNhan(db); db.GiuCho.Add(new GiuCho { IdCaLamViec = ca.IdCaLamViec, IdBenhNhan = benhNhan.IdBenhNhan, SoSlot = 1, GioHetHan = FixedNow.AddMinutes(15), DaGiaiPhong = false, NgayTao = FixedNow }); await db.SaveChangesAsync(); var result = await service.KiemTraSlotTrongAsync(ca.IdCaLamViec, CancellationToken.None); result.CoTheDat.Should().BeFalse(); result.SoGiuChoHieuLuc.Should().Be(1); result.LyDoTuChoi.Should().Be(LyDoKhongDatDuoc.HetSlot); }
    [Fact]
    public async Task IncrementSoSlotDaDatAsync_TangVaGiam_AnToanTheoRangBuoc() { using var factory = new TestDbContextFactory(); var service = CreateService(factory, out var db); var ca = SeedCa(db, TrangThaiDuyetCa.DaDuyet, 5, 1, new DateOnly(2026, 5, 5)); (await service.IncrementSoSlotDaDatAsync(ca.IdCaLamViec, 1, CancellationToken.None)).Should().Be(2); (await service.IncrementSoSlotDaDatAsync(ca.IdCaLamViec, -1, CancellationToken.None)).Should().Be(1); (await service.IncrementSoSlotDaDatAsync(ca.IdCaLamViec, 10, CancellationToken.None)).Should().BeNull(); }
    [Fact]
    public async Task IncrementSoSlotDaDatAsync_KhongDuocAm_ThiTraNull() { using var factory = new TestDbContextFactory(); var service = CreateService(factory, out var db); var ca = SeedCa(db, TrangThaiDuyetCa.DaDuyet, 5, 0, new DateOnly(2026, 5, 5)); (await service.IncrementSoSlotDaDatAsync(ca.IdCaLamViec, -1, CancellationToken.None)).Should().BeNull(); }
    [Fact]
    public async Task IncrementSoSlotDaDatAsync_CaKhongTonTai_TraNull() { using var factory = new TestDbContextFactory(); var service = CreateService(factory, out _); (await service.IncrementSoSlotDaDatAsync(999, 1, CancellationToken.None)).Should().BeNull(); }
    [Fact]
    public async Task ChayReconSlotAsync_DongBoLaiSoSlotDaDat_TheoLichHenVaGiuCho() { using var factory = new TestDbContextFactory(); var service = CreateService(factory, out var db); var ca = SeedCa(db, TrangThaiDuyetCa.DaDuyet, 5, 0, new DateOnly(2026, 5, 5)); var benhNhan = TestDataSeeder.SeedBenhNhan(db); var dichVu = TestDataSeeder.SeedDichVu(db); db.GiuCho.Add(new GiuCho { IdCaLamViec = ca.IdCaLamViec, IdBenhNhan = benhNhan.IdBenhNhan, SoSlot = 1, GioHetHan = FixedNow.AddMinutes(15), DaGiaiPhong = false, NgayTao = FixedNow }); db.LichHen.Add(new EntityLichHen { IdCaLamViec = ca.IdCaLamViec, IdBenhNhan = benhNhan.IdBenhNhan, IdDichVu = dichVu.IdDichVu, SoSlot = 1, HinhThucDat = HinhThucDat.TrucTuyen, TrangThai = TrangThaiLichHen.DangKham, NgayTao = FixedNow, MaLichHen = "LH-UT-1", RowVersion = new byte[] { 0, 0, 0, 0, 0, 0, 0, 1 } }); await db.SaveChangesAsync(); (await service.ChayReconSlotAsync(CancellationToken.None)).Should().Be(1); (await db.CaLamViec.AsNoTracking().SingleAsync(x => x.IdCaLamViec == ca.IdCaLamViec)).SoSlotDaDat.Should().Be(2); }
    [Fact]
    public async Task IncrementSoSlotDaDatAsync_HaiRequestDongThoi_ChiMotRequestThanhCong() { using var factory = new TestDbContextFactory(); using var dbSetup = factory.CreateContext(); var caSeed = SeedCa(dbSetup, TrangThaiDuyetCa.DaDuyet, 1, 0, new DateOnly(2026, 5, 5)); using var db1 = factory.CreateContext(); using var db2 = factory.CreateContext(); var clock1 = Substitute.For<IDateTimeProvider>(); clock1.UtcNow.Returns(FixedNow); var clock2 = Substitute.For<IDateTimeProvider>(); clock2.UtcNow.Returns(FixedNow); var logger1 = Substitute.For<Microsoft.Extensions.Logging.ILogger<CaLamViecQueryService>>(); var logger2 = Substitute.For<Microsoft.Extensions.Logging.ILogger<CaLamViecQueryService>>(); var service1 = new CaLamViecQueryService(db1, clock1, logger1); var service2 = new CaLamViecQueryService(db2, clock2, logger2); var task1 = service1.IncrementSoSlotDaDatAsync(caSeed.IdCaLamViec, 1, CancellationToken.None); var task2 = service2.IncrementSoSlotDaDatAsync(caSeed.IdCaLamViec, 1, CancellationToken.None); await Task.WhenAll(task1.ContinueWith(_ => { }), task2.ContinueWith(_ => { })); var ketQua1 = await task1; var ketQua2 = await task2; new[] { ketQua1, ketQua2 }.Count(x => x.HasValue).Should().Be(1); new[] { ketQua1, ketQua2 }.Count(x => !x.HasValue).Should().Be(1); using var dbCheck = factory.CreateContext(); (await dbCheck.CaLamViec.AsNoTracking().SingleAsync(x => x.IdCaLamViec == caSeed.IdCaLamViec)).SoSlotDaDat.Should().Be(1); }
}
