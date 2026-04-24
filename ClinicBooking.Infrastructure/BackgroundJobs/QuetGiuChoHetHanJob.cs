using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Common.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ClinicBooking.Infrastructure.BackgroundJobs;

/// <summary>
/// Background job: quet va giai phong cac GiuCho het han.
/// Chay theo chu ky <see cref="LichHenOptions.BackgroundJobOptions.QuetGiuChoPhut"/> (mac dinh 1 phut).
/// Khi Module 4 (Hangfire) len, remove AddHostedService va dang ky recurring job tuong duong.
/// </summary>
public sealed class QuetGiuChoHetHanJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<QuetGiuChoHetHanJob> _logger;
    private readonly TimeSpan _chuKy;

    public QuetGiuChoHetHanJob(
        IServiceScopeFactory scopeFactory,
        ILogger<QuetGiuChoHetHanJob> logger,
        IOptions<LichHenOptions> options)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _chuKy = TimeSpan.FromMinutes(options.Value.BackgroundJob.QuetGiuChoPhut);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "[QuetGiuChoHetHanJob] Bat dau. Chu ky: {ChuKy} phut.",
            _chuKy.TotalMinutes);

        using var timer = new PeriodicTimer(_chuKy);

        while (!stoppingToken.IsCancellationRequested
               && await timer.WaitForNextTickAsync(stoppingToken))
        {
            await QuetAsync(stoppingToken);
        }
    }

    /// <summary>Goi noi bo — exposed internal de unit test co the kiem chung logic.</summary>
    internal async Task QuetAsync(CancellationToken ct)
    {
        try
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

            var now = DateTime.UtcNow;

            var soLuong = await db.GiuCho
                .Where(x => !x.DaGiaiPhong && x.GioHetHan < now)
                .ExecuteUpdateAsync(
                    s => s.SetProperty(x => x.DaGiaiPhong, true),
                    ct);

            if (soLuong > 0)
            {
                _logger.LogInformation(
                    "[QuetGiuChoHetHanJob] Da giai phong {SoLuong} giu cho het han luc {Now:HH:mm:ss} UTC.",
                    soLuong, now);
            }
        }
        catch (OperationCanceledException)
        {
            // Dung lai khi host shutdown — hoan toan binh thuong.
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[QuetGiuChoHetHanJob] Loi khi quet giu cho het han.");
        }
    }
}
