using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Entities;
using ClinicBooking.Domain.Enums;
using ClinicBooking.Infrastructure.Persistence;
using ClinicBooking.Infrastructure.Services.Scheduling;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Testcontainers.MsSql;

namespace ClinicBooking.Application.UnitTests.Features.Scheduling.Services;

public sealed class CaLamViecQueryServiceSqlServerTests : IAsyncLifetime
{
    private readonly MsSqlContainer _container = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithPassword("StrongPass!12345")
        .Build();

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }

    [Fact]
    public async Task IncrementSoSlotDaDatAsync_HaiRequestDongThoi_ChiMotRequestThanhCong_TrenSqlServer()
    {
        await using var db = CreateContext();
        SeedCa(db, TrangThaiDuyetCa.DaDuyet, soSlotToiDa: 1, soSlotDaDat: 0, ngay: new DateOnly(2026, 5, 5));

        var clock1 = Substitute.For<IDateTimeProvider>();
        clock1.UtcNow.Returns(new DateTime(2026, 5, 1, 10, 0, 0, DateTimeKind.Utc));
        var clock2 = Substitute.For<IDateTimeProvider>();
        clock2.UtcNow.Returns(new DateTime(2026, 5, 1, 10, 0, 0, DateTimeKind.Utc));

        await using var db1 = CreateContext();
        await using var db2 = CreateContext();
        var service1 = new CaLamViecQueryService(db1, clock1);
        var service2 = new CaLamViecQueryService(db2, clock2);

        var task1 = service1.IncrementSoSlotDaDatAsync(1, 1, CancellationToken.None);
        var task2 = service2.IncrementSoSlotDaDatAsync(1, 1, CancellationToken.None);
        await Task.WhenAll(task1.ContinueWith(_ => { }), task2.ContinueWith(_ => { }));

        var results = new[] { await task1, await task2 };
        results.Count(x => x.HasValue).Should().Be(1);
        results.Count(x => !x.HasValue).Should().Be(1);

        await using var dbCheck = CreateContext();
        (await dbCheck.CaLamViec.AsNoTracking().SingleAsync(x => x.IdCaLamViec == 1)).SoSlotDaDat.Should().Be(1);
    }

    private AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(_container.GetConnectionString())
            .EnableSensitiveDataLogging()
            .Options;

        var context = new AppDbContext(options);
        context.Database.EnsureCreated();
        return context;
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
            NgayTao = DateTime.UtcNow
        });

        db.BacSi.Add(new ClinicBooking.Domain.Entities.BacSi
        {
            IdBacSi = 1,
            IdTaiKhoan = 1,
            IdChuyenKhoa = 1,
            HoTen = "BS SQL",
            LoaiHopDong = LoaiHopDong.NoiTru,
            TrangThai = TrangThaiBacSi.DangLam,
            NgayTao = DateTime.UtcNow
        });

        db.ChuyenKhoa.Add(new ChuyenKhoa { IdChuyenKhoa = 1, TenChuyenKhoa = "CK SQL", ThoiGianSlotMacDinh = 15, HienThi = true });
        db.Phong.Add(new Phong { IdPhong = 1, MaPhong = "P-SQL-1", TenPhong = "Phong SQL", SucChua = 1, TrangThai = true });
        db.DinhNghiaCa.Add(new DinhNghiaCa { IdDinhNghiaCa = 1, TenCa = "Sang", GioBatDauMacDinh = new TimeOnly(8, 0), GioKetThucMacDinh = new TimeOnly(11, 0), TrangThai = true });
        db.TaiKhoan.Add(new TaiKhoan
        {
            IdTaiKhoan = 1,
            TenDangNhap = "sql-user",
            Email = "sql-user@example.com",
            SoDienThoai = "0900000009",
            VaiTro = VaiTro.BacSi,
            TrangThai = true,
            NgayTao = DateTime.UtcNow
        });

        db.SaveChanges();
    }
}
