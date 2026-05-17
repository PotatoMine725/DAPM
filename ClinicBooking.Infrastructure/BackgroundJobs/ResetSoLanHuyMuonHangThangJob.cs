using ClinicBooking.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ClinicBooking.Infrastructure.BackgroundJobs;

/// <summary>
/// Reset so lan huy muon cua benh nhan theo chu ky thang.
/// Chay daily, khi sang thang moi se reset 1 lan.
/// </summary>
public sealed class ResetSoLanHuyMuonHangThangJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ResetSoLanHuyMuonHangThangJob> _logger;
    private readonly TimeProvider _timeProvider;
    private int _thangDaReset = -1;
    private int _namDaReset = -1;

    public ResetSoLanHuyMuonHangThangJob(
        IServiceScopeFactory scopeFactory,
        ILogger<ResetSoLanHuyMuonHangThangJob> logger,
        TimeProvider? timeProvider = null)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _timeProvider = timeProvider ?? TimeProvider.System;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[ResetSoLanHuyMuonHangThangJob] Bat dau.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ChayNeuCanAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ResetSoLanHuyMuonHangThangJob] Loi khi reset so lan huy muon.");
            }

            await Task.Delay(TimeSpan.FromHours(12), stoppingToken);
        }
    }

    private async Task ChayNeuCanAsync(CancellationToken cancellationToken)
    {
        var now = _timeProvider.GetUtcNow().UtcDateTime;
        if (now.Month == _thangDaReset && now.Year == _namDaReset)
        {
            return;
        }

        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

        var danhSach = await db.BenhNhan
            .Where(x => x.SoLanHuyMuon > 0 || x.BiHanChe)
            .ToListAsync(cancellationToken);

        if (danhSach.Count > 0)
        {
            foreach (var benhNhan in danhSach)
            {
                benhNhan.SoLanHuyMuon = 0;
                benhNhan.BiHanChe = false;
                benhNhan.NgayHetHanChe = null;
            }

            await db.SaveChangesAsync(cancellationToken);
        }

        _thangDaReset = now.Month;
        _namDaReset = now.Year;

        _logger.LogInformation(
            "[ResetSoLanHuyMuonHangThangJob] Da reset {SoLuong} benh nhan cho thang {Thang}/{Nam}.",
            danhSach.Count,
            now.Month,
            now.Year);
    }
}
