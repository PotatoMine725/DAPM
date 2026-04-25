using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Common.Options;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Entities;
using ClinicBooking.Infrastructure.BackgroundJobs;
using ClinicBooking.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace ClinicBooking.Application.UnitTests.Features.BackgroundJobs;

public sealed class QuetGiuChoHetHanJobTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly ServiceProvider _serviceProvider;

    public QuetGiuChoHetHanJobTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var services = new ServiceCollection();
        services.AddDbContext<AppDbContext>(o => o.UseSqlite(_connection));
        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());
        _serviceProvider = services.BuildServiceProvider();

        // Tao schema
        using var scope = _serviceProvider.CreateScope();
        scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();
    }

    private QuetGiuChoHetHanJob TaoJob(int quetGiuChoPhut = 1)
    {
        var scopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
        var options = Options.Create(new LichHenOptions
        {
            BackgroundJob = new LichHenOptions.BackgroundJobOptions { QuetGiuChoPhut = quetGiuChoPhut }
        });
        return new QuetGiuChoHetHanJob(scopeFactory, NullLogger<QuetGiuChoHetHanJob>.Instance, options);
    }

    private AppDbContext TaoContext()
    {
        var scope = _serviceProvider.CreateScope();
        return scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    [Fact]
    public async Task QuetAsync_GiuChoHetHan_DanhDauDaGiaiPhong()
    {
        // Arrange
        using var db = TaoContext();
        var bn = TestDataSeeder.SeedBenhNhan(db);
        var ca = TestDataSeeder.SeedCaLamViec(db);

        var giuChoHetHan = new GiuCho
        {
            IdCaLamViec = ca.IdCaLamViec,
            IdBenhNhan = bn.IdBenhNhan,
            SoSlot = 1,
            GioHetHan = DateTime.UtcNow.AddMinutes(-5), // da het han
            DaGiaiPhong = false,
            NgayTao = DateTime.UtcNow.AddMinutes(-20)
        };
        db.GiuCho.Add(giuChoHetHan);
        await db.SaveChangesAsync();

        var job = TaoJob();

        // Act
        await job.QuetAsync(CancellationToken.None);

        // Assert
        using var dbCheck = TaoContext();
        var ketQua = await dbCheck.GiuCho.FindAsync(giuChoHetHan.IdGiuCho);
        ketQua!.DaGiaiPhong.Should().BeTrue();
    }

    [Fact]
    public async Task QuetAsync_GiuChoConHieuLuc_KhongGiaiPhong()
    {
        // Arrange
        using var db = TaoContext();
        var bn = TestDataSeeder.SeedBenhNhan(db);
        var ca = TestDataSeeder.SeedCaLamViec(db);

        var giuChoConHan = new GiuCho
        {
            IdCaLamViec = ca.IdCaLamViec,
            IdBenhNhan = bn.IdBenhNhan,
            SoSlot = 2,
            GioHetHan = DateTime.UtcNow.AddMinutes(10), // con hieu luc
            DaGiaiPhong = false,
            NgayTao = DateTime.UtcNow
        };
        db.GiuCho.Add(giuChoConHan);
        await db.SaveChangesAsync();

        var job = TaoJob();

        // Act
        await job.QuetAsync(CancellationToken.None);

        // Assert
        using var dbCheck = TaoContext();
        var ketQua = await dbCheck.GiuCho.FindAsync(giuChoConHan.IdGiuCho);
        ketQua!.DaGiaiPhong.Should().BeFalse("giu cho con hieu luc khong bi giai phong");
    }

    [Fact]
    public async Task QuetAsync_DaGiaiPhongTuTruoc_KhongBiAnh_Huong()
    {
        // Arrange
        using var db = TaoContext();
        var bn = TestDataSeeder.SeedBenhNhan(db);
        var ca = TestDataSeeder.SeedCaLamViec(db);

        var giuChoDaGiai = new GiuCho
        {
            IdCaLamViec = ca.IdCaLamViec,
            IdBenhNhan = bn.IdBenhNhan,
            SoSlot = 3,
            GioHetHan = DateTime.UtcNow.AddMinutes(-5),
            DaGiaiPhong = true, // da giai phong truoc
            NgayTao = DateTime.UtcNow.AddHours(-1)
        };
        db.GiuCho.Add(giuChoDaGiai);
        await db.SaveChangesAsync();

        var soLuongTruoc = await db.GiuCho.CountAsync(x => x.DaGiaiPhong);
        var job = TaoJob();

        // Act
        await job.QuetAsync(CancellationToken.None);

        // Assert: so luong khong doi
        using var dbCheck = TaoContext();
        var soLuongSau = await dbCheck.GiuCho.CountAsync(x => x.DaGiaiPhong);
        soLuongSau.Should().Be(soLuongTruoc);
    }

    [Fact]
    public async Task QuetAsync_NhieuGiuCho_ChiGiaiPhongHetHan()
    {
        // Arrange
        using var db = TaoContext();
        var bn = TestDataSeeder.SeedBenhNhan(db);
        var ca = TestDataSeeder.SeedCaLamViec(db);

        var hetHan = new GiuCho
        {
            IdCaLamViec = ca.IdCaLamViec, IdBenhNhan = bn.IdBenhNhan, SoSlot = 4,
            GioHetHan = DateTime.UtcNow.AddMinutes(-1), DaGiaiPhong = false,
            NgayTao = DateTime.UtcNow.AddMinutes(-20)
        };
        var conHan = new GiuCho
        {
            IdCaLamViec = ca.IdCaLamViec, IdBenhNhan = bn.IdBenhNhan, SoSlot = 5,
            GioHetHan = DateTime.UtcNow.AddMinutes(10), DaGiaiPhong = false,
            NgayTao = DateTime.UtcNow
        };
        db.GiuCho.AddRange(hetHan, conHan);
        await db.SaveChangesAsync();

        var job = TaoJob();

        // Act
        await job.QuetAsync(CancellationToken.None);

        // Assert
        using var dbCheck = TaoContext();
        var giuChoHetHan = await dbCheck.GiuCho.FindAsync(hetHan.IdGiuCho);
        var giuChoConHan = await dbCheck.GiuCho.FindAsync(conHan.IdGiuCho);
        giuChoHetHan!.DaGiaiPhong.Should().BeTrue();
        giuChoConHan!.DaGiaiPhong.Should().BeFalse();
    }

    public void Dispose()
    {
        _serviceProvider.Dispose();
        _connection.Dispose();
    }
}
