using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Entities;
using ClinicBooking.Domain.Enums;
using ClinicBooking.Infrastructure.Services.Scheduling;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.UnitTests.Features.Scheduling.Services;

public sealed class CaLamViecConflictCheckerTests
{
    [Fact]
    public async Task EnsureKhongXungDotAsync_BacSiTrungCa_ThrowConflict()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var chuyenKhoa = TestDataSeeder.SeedChuyenKhoa(db, "CK Conflict BS");
        var bs = TestDataSeeder.SeedBacSi(db, chuyenKhoa.IdChuyenKhoa);
        var phong1 = TestDataSeeder.SeedPhong(db);
        var phong2 = TestDataSeeder.SeedPhong(db);
        var dnCa = TestDataSeeder.SeedDinhNghiaCa(db);

        db.CaLamViec.Add(new CaLamViec
        {
            IdBacSi = bs.IdBacSi,
            IdPhong = phong1.IdPhong,
            IdChuyenKhoa = chuyenKhoa.IdChuyenKhoa,
            IdDinhNghiaCa = dnCa.IdDinhNghiaCa,
            NgayLamViec = new DateOnly(2026, 5, 5),
            GioBatDau = new TimeOnly(8, 0),
            GioKetThuc = new TimeOnly(11, 0),
            ThoiGianSlot = 15,
            SoSlotToiDa = 10,
            SoSlotDaDat = 0,
            TrangThaiDuyet = TrangThaiDuyetCa.DaDuyet,
            NguonTaoCa = NguonTaoCa.TuDong,
            NgayTao = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        var checker = new CaLamViecConflictChecker(db);

        var act = () => checker.EnsureKhongXungDotAsync(
            bs.IdBacSi,
            phong2.IdPhong,
            new DateOnly(2026, 5, 5),
            new TimeOnly(9, 0),
            new TimeOnly(12, 0),
            null,
            CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("*Bác sĩ*")
            .WithMessage("*đã có ca khác lúc*")
            .WithMessage("*ngày 05/05/2026*");
    }

    [Fact]
    public async Task EnsureKhongXungDotAsync_PhongTrungCa_ThrowConflict()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var chuyenKhoa = TestDataSeeder.SeedChuyenKhoa(db, "CK Conflict Room");
        var bs1 = TestDataSeeder.SeedBacSi(db, chuyenKhoa.IdChuyenKhoa);
        var bs2 = TestDataSeeder.SeedBacSi(db, chuyenKhoa.IdChuyenKhoa);
        var phong = TestDataSeeder.SeedPhong(db);
        var dnCa = TestDataSeeder.SeedDinhNghiaCa(db);

        db.CaLamViec.Add(new CaLamViec
        {
            IdBacSi = bs1.IdBacSi,
            IdPhong = phong.IdPhong,
            IdChuyenKhoa = chuyenKhoa.IdChuyenKhoa,
            IdDinhNghiaCa = dnCa.IdDinhNghiaCa,
            NgayLamViec = new DateOnly(2026, 5, 5),
            GioBatDau = new TimeOnly(8, 0),
            GioKetThuc = new TimeOnly(11, 0),
            ThoiGianSlot = 15,
            SoSlotToiDa = 10,
            SoSlotDaDat = 0,
            TrangThaiDuyet = TrangThaiDuyetCa.DaDuyet,
            NguonTaoCa = NguonTaoCa.TuDong,
            NgayTao = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        var checker = new CaLamViecConflictChecker(db);

        var act = () => checker.EnsureKhongXungDotAsync(
            bs2.IdBacSi,
            phong.IdPhong,
            new DateOnly(2026, 5, 5),
            new TimeOnly(9, 0),
            new TimeOnly(12, 0),
            null,
            CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("*Phòng*")
            .WithMessage("*đã được đặt lúc*")
            .WithMessage("*ngày 05/05/2026*");
    }

    [Fact]
    public async Task EnsureKhongXungDotAsync_CoDonNghiPhepDuyet_ThrowConflict()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var chuyenKhoa = TestDataSeeder.SeedChuyenKhoa(db, "CK Conflict Leave");
        var bs = TestDataSeeder.SeedBacSi(db, chuyenKhoa.IdChuyenKhoa);
        var phong = TestDataSeeder.SeedPhong(db);
        var dnCa = TestDataSeeder.SeedDinhNghiaCa(db);

        var caNghiPhep = new CaLamViec
        {
            IdBacSi = bs.IdBacSi,
            IdPhong = phong.IdPhong,
            IdChuyenKhoa = chuyenKhoa.IdChuyenKhoa,
            IdDinhNghiaCa = dnCa.IdDinhNghiaCa,
            NgayLamViec = new DateOnly(2026, 5, 5),
            GioBatDau = new TimeOnly(8, 0),
            GioKetThuc = new TimeOnly(11, 0),
            ThoiGianSlot = 15,
            SoSlotToiDa = 10,
            SoSlotDaDat = 0,
            TrangThaiDuyet = TrangThaiDuyetCa.DaDuyet,
            NguonTaoCa = NguonTaoCa.TuDong,
            NgayTao = DateTime.UtcNow
        };
        db.CaLamViec.Add(caNghiPhep);
        await db.SaveChangesAsync();

        db.DonNghiPhep.Add(new DonNghiPhep
        {
            IdBacSi = bs.IdBacSi,
            IdCaLamViec = caNghiPhep.IdCaLamViec,
            LoaiNghiPhep = LoaiNghiPhep.CoKeHoach,
            LyDo = "Nghi benh",
            TrangThaiDuyet = TrangThaiDuyetDon.DaDuyet,
            NgayGuiDon = DateTime.UtcNow,
            NgayXuLy = DateTime.UtcNow,
            IdNguoiDuyet = 1
        });
        await db.SaveChangesAsync();

        var checker = new CaLamViecConflictChecker(db);

        var act = () => checker.EnsureKhongXungDotAsync(
            bs.IdBacSi,
            phong.IdPhong,
            new DateOnly(2026, 5, 5),
            new TimeOnly(9, 0),
            new TimeOnly(12, 0),
            null,
            CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("*Bác sĩ*")
            .WithMessage("*đang nghỉ phép*")
            .WithMessage("*trong khung giờ*");
    }
}
