using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Common.Options;
using ClinicBooking.Domain.Entities;
using ClinicBooking.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ClinicBooking.Infrastructure.BackgroundJobs;

/// <summary>
/// Background job: chuyen cac LichHen chua giai quyet sang trang thai DaQuaHan
/// khi ca lam viec da ket thuc vuot qua nguong buffer.
/// Chay theo chu ky <see cref="LichHenOptions.BackgroundJobOptions.ChuyenDaQuaHanPhut"/> (mac dinh 30 phut).
/// Khi Module 4 (Hangfire) len, remove AddHostedService va dang ky recurring job tuong duong.
/// </summary>
public sealed class ChuyenLichHenDaQuaHanJob : BackgroundService
{
    private static readonly TrangThaiLichHen[] TrangThaiChuaGiaiQuyet =
    [
        TrangThaiLichHen.ChoXacNhan,
        TrangThaiLichHen.DaXacNhan
    ];

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ChuyenLichHenDaQuaHanJob> _logger;
    private readonly TimeSpan _chuKy;
    private readonly int _bufferGio;

    public ChuyenLichHenDaQuaHanJob(
        IServiceScopeFactory scopeFactory,
        ILogger<ChuyenLichHenDaQuaHanJob> logger,
        IOptions<LichHenOptions> options)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _chuKy = TimeSpan.FromMinutes(options.Value.BackgroundJob.ChuyenDaQuaHanPhut);
        _bufferGio = options.Value.BackgroundJob.BufferSauKetThucGio;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "[ChuyenLichHenDaQuaHanJob] Bat dau. Chu ky: {ChuKy} phut, buffer: {Buffer} gio.",
            _chuKy.TotalMinutes, _bufferGio);

        using var timer = new PeriodicTimer(_chuKy);

        while (!stoppingToken.IsCancellationRequested
               && await timer.WaitForNextTickAsync(stoppingToken))
        {
            await ChuyenAsync(stoppingToken);
        }
    }

    /// <summary>Goi noi bo — exposed internal de unit test co the kiem chung logic.</summary>
    internal async Task ChuyenAsync(CancellationToken ct)
    {
        try
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<IAppDbContext>();

            // Lay gio hien tai theo local time (VN) de so sanh voi NgayLamViec + GioKetThuc
            // Ca lam viec duoc luu theo gio dia phuong, dung DateTime.Now de tranh lech timezone.
            var now = DateTime.Now;
            var nguong = now.AddHours(-_bufferGio);

            // Buoc 1: pre-filter Ca theo ngay (tra ve nho) roi filter chinh xac phia client
            // Tranh dung DateOnly.ToDateTime() trong EF LINQ vi SQLite/SQL Server dich khac nhau.
            var ngayNguong = DateOnly.FromDateTime(nguong);
            var caUngVien = await db.CaLamViec
                .AsNoTracking()
                .Where(c => c.NgayLamViec <= ngayNguong)
                .Select(c => new { c.IdCaLamViec, c.NgayLamViec, c.GioKetThuc })
                .ToListAsync(ct);

            // Loc chinh xac phia client: ca ket thuc truoc nguong
            var caQuaHanIds = caUngVien
                .Where(c => c.NgayLamViec.ToDateTime(c.GioKetThuc) < nguong)
                .Select(c => c.IdCaLamViec)
                .ToHashSet();

            if (caQuaHanIds.Count == 0)
                return;

            // Buoc 2: tim lich hen chua giai quyet trong cac ca da qua han
            var danhSachId = await db.LichHen
                .AsNoTracking()
                .Where(lh =>
                    TrangThaiChuaGiaiQuyet.Contains(lh.TrangThai)
                    && caQuaHanIds.Contains(lh.IdCaLamViec))
                .Select(lh => lh.IdLichHen)
                .ToListAsync(ct);

            if (danhSachId.Count == 0)
                return;

            _logger.LogInformation(
                "[ChuyenLichHenDaQuaHanJob] Tim thay {SoLuong} lich hen can chuyen sang DaQuaHan.",
                danhSachId.Count);

            // Cap nhat trang thai
            var soCapNhat = await db.LichHen
                .Where(lh => danhSachId.Contains(lh.IdLichHen))
                .ExecuteUpdateAsync(
                    s => s.SetProperty(x => x.TrangThai, TrangThaiLichHen.DaQuaHan),
                    ct);

            // Ghi lich su cho tung lich hen
            var danhSachLichSu = danhSachId.Select(id => new LichSuLichHen
            {
                IdLichHen = id,
                HanhDong = HanhDongLichSu.QuaHan,
                LyDo = "Ca lam viec da ket thuc, lich hen tu dong chuyen sang qua han.",
                NgayTao = DateTime.UtcNow
            }).ToList();

            db.LichSuLichHen.AddRange(danhSachLichSu);
            await db.SaveChangesAsync(ct);

            _logger.LogInformation(
                "[ChuyenLichHenDaQuaHanJob] Da chuyen {SoCapNhat} lich hen sang DaQuaHan, ghi {SoLichSu} lich su.",
                soCapNhat, danhSachLichSu.Count);
        }
        catch (OperationCanceledException)
        {
            // Dung lai khi host shutdown — hoan toan binh thuong.
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[ChuyenLichHenDaQuaHanJob] Loi khi chuyen lich hen sang DaQuaHan.");
        }
    }
}
