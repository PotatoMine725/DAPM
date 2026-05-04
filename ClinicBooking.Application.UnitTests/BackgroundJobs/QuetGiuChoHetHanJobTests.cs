using ClinicBooking.Application.Common.Options;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Entities;
using ClinicBooking.Infrastructure.BackgroundJobs;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace ClinicBooking.Application.UnitTests.BackgroundJobs;

public sealed class QuetGiuChoHetHanJobTests
{
    private static IOptions<LichHenOptions> DefaultOptions() =>
        Options.Create(new LichHenOptions
        {
            BackgroundJob = new LichHenOptions.BackgroundJobOptions { QuetGiuChoPhut = 1 }
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

    [Fact]
    public async Task QuetAsync_GiuChoHetHan_DanhDauDaGiaiPhong()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var bn = TestDataSeeder.SeedBenhNhan(db);
        var ca = TestDataSeeder.SeedCaLamViec(db);
        db.GiuCho.Add(new GiuCho
        {
            IdCaLamViec = ca.IdCaLamViec,
            IdBenhNhan = bn.IdBenhNhan,
            SoSlot = 1,
            GioHetHan = DateTime.UtcNow.AddMinutes(-5),
            DaGiaiPhong = false,
            NgayTao = DateTime.UtcNow.AddMinutes(-20)
        });
        db.SaveChanges();

        var job = new QuetGiuChoHetHanJob(
            CreateScopeFactory(db),
            Substitute.For<ILogger<QuetGiuChoHetHanJob>>(),
            DefaultOptions());

        await job.QuetAsync(CancellationToken.None);

        var giuCho = await db.GiuCho.AsNoTracking().FirstAsync();
        giuCho.DaGiaiPhong.Should().BeTrue();
    }

    [Fact]
    public async Task QuetAsync_GiuChoConHan_KhongThayDoi()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var bn = TestDataSeeder.SeedBenhNhan(db);
        var ca = TestDataSeeder.SeedCaLamViec(db);
        db.GiuCho.Add(new GiuCho
        {
            IdCaLamViec = ca.IdCaLamViec,
            IdBenhNhan = bn.IdBenhNhan,
            SoSlot = 1,
            GioHetHan = DateTime.UtcNow.AddMinutes(+5),
            DaGiaiPhong = false,
            NgayTao = DateTime.UtcNow.AddMinutes(-1)
        });
        db.SaveChanges();

        var job = new QuetGiuChoHetHanJob(
            CreateScopeFactory(db),
            Substitute.For<ILogger<QuetGiuChoHetHanJob>>(),
            DefaultOptions());

        await job.QuetAsync(CancellationToken.None);

        var giuCho = await db.GiuCho.AsNoTracking().FirstAsync();
        giuCho.DaGiaiPhong.Should().BeFalse();
    }
}
