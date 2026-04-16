using ClinicBooking.Application.Abstractions.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ClinicBooking.Infrastructure.Persistence;

/// <summary>
/// Chay mot lan khi app khoi dong de dong bo cac du lieu can "sua sau migration".
/// Hien tai: thay hash mat khau admin gia lap (tu migration) bang hash BCrypt that
/// dua tren cau hinh Admin:MatKhauMacDinh. Hoat dong idempotent — chay lai khong reset.
/// </summary>
public class DatabaseSeeder
{
    private const string TenDangNhapAdmin = "admin";

    private readonly AppDbContext _db;
    private readonly IPasswordHasher _passwordHasher;
    private readonly AdminSeederSettings _settings;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(
        AppDbContext db,
        IPasswordHasher passwordHasher,
        IOptions<AdminSeederSettings> settings,
        ILogger<DatabaseSeeder> logger)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await FixMatKhauAdminAsync(cancellationToken);
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
            // Da duoc fix tu truoc hoac admin da doi mat khau — khong dong vao.
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
}
