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

        await SeedTaiKhoanFixtureAsync(fixture.BenhNhan, VaiTro.BenhNhan, matKhau, cancellationToken);
        await SeedTaiKhoanFixtureAsync(fixture.BacSi, VaiTro.BacSi, matKhau, cancellationToken);
        await SeedTaiKhoanFixtureAsync(fixture.LeTan, VaiTro.LeTan, matKhau, cancellationToken);
        await SeedTaiKhoanFixtureAsync(fixture.Admin, VaiTro.Admin, matKhau, cancellationToken);
    }

    private async Task SeedTaiKhoanFixtureAsync(
        FixtureTaiKhoan? config,
        VaiTro vaiTro,
        string matKhau,
        CancellationToken cancellationToken)
    {
        if (config is null || string.IsNullOrWhiteSpace(config.TenDangNhap))
        {
            return;
        }

        var now = _dateTimeProvider.UtcNow;
        var taiKhoan = await _db.TaiKhoan
            .FirstOrDefaultAsync(x => x.TenDangNhap == config.TenDangNhap, cancellationToken);

        var isNewTaiKhoan = taiKhoan is null;
        if (isNewTaiKhoan)
        {
            taiKhoan = new TaiKhoan
            {
                TenDangNhap = config.TenDangNhap,
                Email = config.Email,
                SoDienThoai = config.SoDienThoai,
                MatKhau = _passwordHasher.HashPassword(matKhau),
                VaiTro = vaiTro,
                TrangThai = true,
                NgayTao = now
            };
            _db.TaiKhoan.Add(taiKhoan);
            await _db.SaveChangesAsync(cancellationToken);
        }
        else
        {
            taiKhoan.Email = config.Email;
            taiKhoan.SoDienThoai = config.SoDienThoai;
            taiKhoan.MatKhau = _passwordHasher.HashPassword(matKhau);
            taiKhoan.VaiTro = vaiTro;
            taiKhoan.TrangThai = true;
            await _db.SaveChangesAsync(cancellationToken);
        }

        switch (vaiTro)
        {
            case VaiTro.BenhNhan:
            {
                var benhNhan = await _db.BenhNhan.FirstOrDefaultAsync(x => x.IdTaiKhoan == taiKhoan.IdTaiKhoan, cancellationToken);
                if (benhNhan is null)
                {
                    _db.BenhNhan.Add(new BenhNhan
                    {
                        IdTaiKhoan = taiKhoan.IdTaiKhoan,
                        HoTen = string.IsNullOrWhiteSpace(config.HoTen) ? config.TenDangNhap : config.HoTen,
                        NgayTao = now
                    });
                }
                else
                {
                    benhNhan.HoTen = string.IsNullOrWhiteSpace(config.HoTen) ? config.TenDangNhap : config.HoTen;
                    benhNhan.NgayTao = benhNhan.NgayTao == default ? now : benhNhan.NgayTao;
                }
                break;
            }
            case VaiTro.BacSi:
            {
                var chuyenKhoa = await _db.ChuyenKhoa
                    .OrderBy(x => x.IdChuyenKhoa)
                    .FirstOrDefaultAsync(cancellationToken);
                if (chuyenKhoa is null)
                {
                    _logger.LogWarning("Khong co chuyen khoa nao de seed bac si fixture '{Ten}'.", config.TenDangNhap);
                    return;
                }

                var bacSi = await _db.BacSi.FirstOrDefaultAsync(x => x.IdTaiKhoan == taiKhoan.IdTaiKhoan, cancellationToken);
                if (bacSi is null)
                {
                    _db.BacSi.Add(new BacSi
                    {
                        IdTaiKhoan = taiKhoan.IdTaiKhoan,
                        IdChuyenKhoa = chuyenKhoa.IdChuyenKhoa,
                        HoTen = string.IsNullOrWhiteSpace(config.HoTen) ? config.TenDangNhap : config.HoTen,
                        LoaiHopDong = LoaiHopDong.NoiTru,
                        TrangThai = TrangThaiBacSi.DangLam,
                        NgayTao = now
                    });
                }
                else
                {
                    bacSi.IdChuyenKhoa = chuyenKhoa.IdChuyenKhoa;
                    bacSi.HoTen = string.IsNullOrWhiteSpace(config.HoTen) ? config.TenDangNhap : config.HoTen;
                    bacSi.LoaiHopDong = LoaiHopDong.NoiTru;
                    bacSi.TrangThai = TrangThaiBacSi.DangLam;
                }
                break;
            }
            case VaiTro.LeTan:
            {
                var leTan = await _db.LeTan.FirstOrDefaultAsync(x => x.IdTaiKhoan == taiKhoan.IdTaiKhoan, cancellationToken);
                if (leTan is null)
                {
                    _db.LeTan.Add(new LeTan
                    {
                        IdTaiKhoan = taiKhoan.IdTaiKhoan,
                        HoTen = string.IsNullOrWhiteSpace(config.HoTen) ? config.TenDangNhap : config.HoTen,
                        NgayTao = now
                    });
                }
                else
                {
                    leTan.HoTen = string.IsNullOrWhiteSpace(config.HoTen) ? config.TenDangNhap : config.HoTen;
                }
                break;
            }
            case VaiTro.Admin:
                // Admin fixture chi can tai khoan; khong can profile rieng.
                break;
        }

        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogWarning(
            "[DevFixture] Da upsert tai khoan fixture '{Ten}' vai tro {VaiTro} voi mat khau cau hinh. CHI dung o Development.",
            config.TenDangNhap,
            vaiTro);
    }
}
