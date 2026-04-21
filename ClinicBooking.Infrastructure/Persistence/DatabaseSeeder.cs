using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Domain.Entities;
using ClinicBooking.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ClinicBooking.Infrastructure.Persistence;

/// <summary>
/// Chay mot lan khi app khoi dong de dong bo cac du lieu can "sua sau migration".
/// - Thay hash mat khau admin gia lap (tu migration) bang hash BCrypt that dua tren
///   cau hinh Admin:MatKhauMacDinh.
/// - Neu bat Admin:DevFixture:Enabled, seed them cac tai khoan fixture (le_tan, ...).
/// Hoat dong idempotent: chay lai nhieu lan khong reset du lieu da co.
/// </summary>
public class DatabaseSeeder
{
    private const string TenDangNhapAdmin = "admin";

    private readonly AppDbContext _db;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly AdminSeederSettings _settings;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(
        AppDbContext db,
        IPasswordHasher passwordHasher,
        IDateTimeProvider dateTimeProvider,
        IOptions<AdminSeederSettings> settings,
        ILogger<DatabaseSeeder> logger)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _dateTimeProvider = dateTimeProvider;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await FixMatKhauAdminAsync(cancellationToken);
        await SeedDevFixtureAsync(cancellationToken);
    }

    private async Task FixMatKhauAdminAsync(CancellationToken cancellationToken)
    {
        var admin = await _db.TaiKhoan
            .FirstOrDefaultAsync(x => x.TenDangNhap == TenDangNhapAdmin, cancellationToken);

        if (admin is null)
        {
            _logger.LogWarning(
                "Khong tim thay tai khoan admin. Hay chay 'dotnet ef database update' truoc khi khoi dong app.");
            return;
        }

        if (admin.MatKhau != SeedData.HashMatKhauAdminGiaLap)
        {
            _logger.LogDebug("Mat khau admin da duoc dat tu truoc. Bo qua seeding.");
            return;
        }

        if (string.IsNullOrWhiteSpace(_settings.MatKhauMacDinh))
        {
            _logger.LogWarning(
                "Tai khoan admin van con hash gia lap nhung khong co cau hinh Admin:MatKhauMacDinh. " +
                "Them cau hinh nay vao appsettings.Development.json, User Secrets, hoac bien moi truong " +
                "de fix mat khau admin.");
            return;
        }

        admin.MatKhau = _passwordHasher.HashPassword(_settings.MatKhauMacDinh);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogWarning(
            "Da thay hash gia lap cua tai khoan admin bang BCrypt hash that. " +
            "KHUYEN CAO: dang nhap va doi mat khau admin ngay lap tuc.");
    }

    private async Task SeedDevFixtureAsync(CancellationToken cancellationToken)
    {
        var fixture = _settings.DevFixture;
        if (fixture is null || !fixture.Enabled)
        {
            return;
        }

        var matKhau = !string.IsNullOrWhiteSpace(fixture.MatKhauChung)
            ? fixture.MatKhauChung
            : _settings.MatKhauMacDinh;

        if (string.IsNullOrWhiteSpace(matKhau))
        {
            _logger.LogWarning(
                "DevFixture dang bat nhung khong co mat khau (DevFixture:MatKhauChung / Admin:MatKhauMacDinh). " +
                "Bo qua seed fixture.");
            return;
        }

        await SeedLeTanFixtureAsync(fixture.LeTan, matKhau, cancellationToken);
    }

    private async Task SeedLeTanFixtureAsync(
        FixtureTaiKhoan? config,
        string matKhau,
        CancellationToken cancellationToken)
    {
        if (config is null || string.IsNullOrWhiteSpace(config.TenDangNhap))
        {
            return;
        }

        var daCo = await _db.TaiKhoan
            .AnyAsync(x => x.TenDangNhap == config.TenDangNhap, cancellationToken);
        if (daCo)
        {
            _logger.LogDebug("Tai khoan le_tan fixture '{Ten}' da ton tai. Bo qua.", config.TenDangNhap);
            return;
        }

        var now = _dateTimeProvider.UtcNow;
        var taiKhoan = new TaiKhoan
        {
            TenDangNhap = config.TenDangNhap,
            Email = config.Email,
            SoDienThoai = config.SoDienThoai,
            MatKhau = _passwordHasher.HashPassword(matKhau),
            VaiTro = VaiTro.LeTan,
            TrangThai = true,
            NgayTao = now
        };
        _db.TaiKhoan.Add(taiKhoan);
        await _db.SaveChangesAsync(cancellationToken);

        _db.LeTan.Add(new LeTan
        {
            IdTaiKhoan = taiKhoan.IdTaiKhoan,
            HoTen = string.IsNullOrWhiteSpace(config.HoTen) ? config.TenDangNhap : config.HoTen,
            NgayTao = now
        });
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogWarning(
            "[DevFixture] Da seed tai khoan le_tan '{Ten}' voi mat khau cau hinh. " +
            "CHI dung o moi truong Development.",
            config.TenDangNhap);
    }
}
