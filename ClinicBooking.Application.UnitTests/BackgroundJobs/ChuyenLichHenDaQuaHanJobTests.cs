using ClinicBooking.Application.Common.Options;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Enums;
using ClinicBooking.Infrastructure.BackgroundJobs;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace ClinicBooking.Application.UnitTests.BackgroundJobs;

public sealed class ChuyenLichHenDaQuaHanJobTests
{
    // Buffer=0 nen nguong = DateTime.Now, giup test dat duoc ca ket thuc truoc Now
    private static IOptions<LichHenOptions> DefaultOptions(int bufferGio = 0) =>
        Options.Create(new LichHenOptions
        {
            BackgroundJob = new LichHenOptions.BackgroundJobOptions
            {
                ChuyenDaQuaHanPhut = 30,
                BufferSauKetThucGio = bufferGio
            }
        });

    private static IServiceScopeFactory CreateScopeFactory(ClinicBooking.Infrastructure.Persistence.AppDbContext db)
    {
        var serviceProvider = Substitute.For<IServiceProvider>();
        serviceProvider.GetService(typeof(ClinicBooking.Application.Abstractions.Persistence.IAppDbContext))
            .Returns(db);

        var scope = Substitute.For<IServiceScope, IAsyncDisposable>();
        scope.ServiceProvider.Returns(serviceProvider);

        var scopeFactory = Substitute.For<IServiceScopeFactory>();
        scopeFactory.CreateScope().Returns(scope);

        return scopeFactory;
    }

    // Seed CaLamViec ket thuc 2 ngay truoc (chac chan qua han voi moi timezone)
    private static DateOnly NgayLamViecQuaKhu() =>
        DateOnly.FromDateTime(DateTime.Now).AddDays(-2);

    [Fact]
    public async Task ChuyenAsync_LichChoXacNhanCaKetThuc_ChuyenThanhDaQuaHan()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var bn = TestDataSeeder.SeedBenhNhan(db);
        // Ca ket thuc luc 11:00, 2 ngay truoc → chac chan qua han
        var ca = TestDataSeeder.SeedCaLamViec(db, ngayLamViec: NgayLamViecQuaKhu());
        var lh = TestDataSeeder.SeedLichHen(db, bn.IdBenhNhan, ca.IdCaLamViec,
            trangThai: TrangThaiLichHen.ChoXacNhan);

        var job = new ChuyenLichHenDaQuaHanJob(
            CreateScopeFactory(db),
            Substitute.For<ILogger<ChuyenLichHenDaQuaHanJob>>(),
            DefaultOptions(bufferGio: 0));

        await job.ChuyenAsync(CancellationToken.None);

        var sau = await db.LichHen.AsNoTracking().FirstAsync(x => x.IdLichHen == lh.IdLichHen);
        sau.TrangThai.Should().Be(TrangThaiLichHen.DaQuaHan);

        var lichSuCoQuaHan = await db.LichSuLichHen.AsNoTracking()
            .AnyAsync(x => x.IdLichHen == lh.IdLichHen && x.HanhDong == HanhDongLichSu.QuaHan);
        lichSuCoQuaHan.Should().BeTrue();
    }

    [Fact]
    public async Task ChuyenAsync_LichHoanThanh_KhongBiThayDoi()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var bn = TestDataSeeder.SeedBenhNhan(db);
        var ca = TestDataSeeder.SeedCaLamViec(db, ngayLamViec: NgayLamViecQuaKhu());
        var lh = TestDataSeeder.SeedLichHen(db, bn.IdBenhNhan, ca.IdCaLamViec,
            trangThai: TrangThaiLichHen.HoanThanh);

        var job = new ChuyenLichHenDaQuaHanJob(
            CreateScopeFactory(db),
            Substitute.For<ILogger<ChuyenLichHenDaQuaHanJob>>(),
            DefaultOptions(bufferGio: 0));

        await job.ChuyenAsync(CancellationToken.None);

        var sau = await db.LichHen.AsNoTracking().FirstAsync(x => x.IdLichHen == lh.IdLichHen);
        sau.TrangThai.Should().Be(TrangThaiLichHen.HoanThanh);
    }
}
