using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Common.Options;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Enums;
using ClinicBooking.Infrastructure.BackgroundJobs;
using ClinicBooking.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace ClinicBooking.Application.UnitTests.Features.BackgroundJobs;

public sealed class ChuyenLichHenDaQuaHanJobTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly ServiceProvider _serviceProvider;

    public ChuyenLichHenDaQuaHanJobTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var services = new ServiceCollection();
        services.AddDbContext<AppDbContext>(o => o.UseSqlite(_connection));
        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());
        _serviceProvider = services.BuildServiceProvider();

        using var scope = _serviceProvider.CreateScope();
        scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();
    }

    private ChuyenLichHenDaQuaHanJob TaoJob(int bufferGio = 1)
    {
        var scopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
        var options = Options.Create(new LichHenOptions
        {
            BackgroundJob = new LichHenOptions.BackgroundJobOptions
            {
                ChuyenDaQuaHanPhut = 30,
                BufferSauKetThucGio = bufferGio
            }
        });
        return new ChuyenLichHenDaQuaHanJob(scopeFactory, NullLogger<ChuyenLichHenDaQuaHanJob>.Instance, options);
    }

    private AppDbContext TaoContext()
    {
        var scope = _serviceProvider.CreateScope();
        return scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    /// <summary>Tao CaLamViec voi GioKetThuc trong qua khu (local time).</summary>
    private static DateOnly NgayQuaKhu(int soGioTruoc = 3)
        => DateOnly.FromDateTime(DateTime.Now.AddHours(-soGioTruoc));

    [Fact]
    public async Task ChuyenAsync_LichHenChoXacNhanCaQuaHan_ChuyenSangDaQuaHan()
    {
        // Arrange: ca ket thuc 3 gio truoc (> buffer 1h)
        using var db = TaoContext();
        var bn = TestDataSeeder.SeedBenhNhan(db);

        var ngayQuaKhu = NgayQuaKhu(soGioTruoc: 3);
        var ca = TestDataSeeder.SeedCaLamViec(
            db,
            ngayLamViec: ngayQuaKhu,
            gioBatDau: new TimeOnly(0, 1));      // bat dau 00:01

        // Override GioKetThuc = 00:30 (van trong cung ngay qua khu, da het)
        ca.GioKetThuc = new TimeOnly(0, 30);
        await db.SaveChangesAsync();

        var dv = TestDataSeeder.SeedDichVu(db);
        var lh = TestDataSeeder.SeedLichHen(db, bn.IdBenhNhan, ca.IdCaLamViec,
            idDichVu: dv.IdDichVu,
            trangThai: TrangThaiLichHen.ChoXacNhan);
        var job = TaoJob(bufferGio: 1);

        // Act
        await job.ChuyenAsync(CancellationToken.None);

        // Assert trang thai
        using var dbCheck = TaoContext();
        var lichHen = await dbCheck.LichHen.FindAsync(lh.IdLichHen);
        lichHen!.TrangThai.Should().Be(TrangThaiLichHen.DaQuaHan);

        // Assert lich su
        var lichSu = await dbCheck.LichSuLichHen
            .FirstOrDefaultAsync(ls => ls.IdLichHen == lh.IdLichHen
                                       && ls.HanhDong == HanhDongLichSu.QuaHan);
        lichSu.Should().NotBeNull("phai ghi LichSuLichHen.QuaHan");
    }

    [Fact]
    public async Task ChuyenAsync_LichHenDaXacNhanCaQuaHan_ChuyenSangDaQuaHan()
    {
        // Arrange: trang thai DaXacNhan — cung phai bi chuyen
        using var db = TaoContext();
        var bn = TestDataSeeder.SeedBenhNhan(db);
        var ngayQuaKhu = NgayQuaKhu(soGioTruoc: 3);
        var ca = TestDataSeeder.SeedCaLamViec(db, ngayLamViec: ngayQuaKhu,
            gioBatDau: new TimeOnly(0, 1));
        ca.GioKetThuc = new TimeOnly(0, 30);
        await db.SaveChangesAsync();

        var dv = TestDataSeeder.SeedDichVu(db);
        var lh = TestDataSeeder.SeedLichHen(db, bn.IdBenhNhan, ca.IdCaLamViec,
            idDichVu: dv.IdDichVu,
            trangThai: TrangThaiLichHen.DaXacNhan);
        var job = TaoJob(bufferGio: 1);

        // Act
        await job.ChuyenAsync(CancellationToken.None);

        // Assert
        using var dbCheck = TaoContext();
        var lichHen = await dbCheck.LichHen.FindAsync(lh.IdLichHen);
        lichHen!.TrangThai.Should().Be(TrangThaiLichHen.DaQuaHan);
    }

    [Fact]
    public async Task ChuyenAsync_LichHenCaConHieuLuc_KhongThayDoi()
    {
        // Arrange: ca lam viec 2 ngay sau (chua ket thuc)
        using var db = TaoContext();
        var bn = TestDataSeeder.SeedBenhNhan(db);
        var ngayTuong_lai = DateOnly.FromDateTime(DateTime.Now.AddDays(2));
        var ca = TestDataSeeder.SeedCaLamViec(db, ngayLamViec: ngayTuong_lai);

        var dv = TestDataSeeder.SeedDichVu(db);
        var lh = TestDataSeeder.SeedLichHen(db, bn.IdBenhNhan, ca.IdCaLamViec,
            idDichVu: dv.IdDichVu,
            trangThai: TrangThaiLichHen.ChoXacNhan);
        var job = TaoJob();

        // Act
        await job.ChuyenAsync(CancellationToken.None);

        // Assert: khong bi chuyen
        using var dbCheck = TaoContext();
        var lichHen = await dbCheck.LichHen.FindAsync(lh.IdLichHen);
        lichHen!.TrangThai.Should().Be(TrangThaiLichHen.ChoXacNhan);
    }

    [Fact]
    public async Task ChuyenAsync_LichHenDaHuy_KhongThayDoi()
    {
        // Arrange: trang thai HuyBenhNhan — khong nam trong danh sach chuyen
        using var db = TaoContext();
        var bn = TestDataSeeder.SeedBenhNhan(db);
        var ngayQuaKhu = NgayQuaKhu(soGioTruoc: 3);
        var ca = TestDataSeeder.SeedCaLamViec(db, ngayLamViec: ngayQuaKhu,
            gioBatDau: new TimeOnly(0, 1));
        ca.GioKetThuc = new TimeOnly(0, 30);
        await db.SaveChangesAsync();

        var dv = TestDataSeeder.SeedDichVu(db);
        var lh = TestDataSeeder.SeedLichHen(db, bn.IdBenhNhan, ca.IdCaLamViec,
            idDichVu: dv.IdDichVu,
            trangThai: TrangThaiLichHen.HuyBenhNhan);
        var job = TaoJob();

        // Act
        await job.ChuyenAsync(CancellationToken.None);

        // Assert: van giu trang thai cu
        using var dbCheck = TaoContext();
        var lichHen = await dbCheck.LichHen.FindAsync(lh.IdLichHen);
        lichHen!.TrangThai.Should().Be(TrangThaiLichHen.HuyBenhNhan);
    }

    public void Dispose()
    {
        _serviceProvider.Dispose();
        _connection.Dispose();
    }
}
