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
        await SyncCaLamViecVaLichHenDemoAsync(cancellationToken);
    }

    // CaLamViec IDs 3001-3005 la du lieu seed demo.
    // 3001/3002 → ngay mai (today+1), 3003 → tuan sau (today+7), 3004/3005 → hom nay (today).
    private static readonly int[] SeededCaLamViecIds = [3001, 3002, 3003, 3004, 3005];

    // LichHen co MaLichHen bat dau bang "DEMO-" la du lieu demo, bi xoa va tao lai moi startup.
    // LichHen IDs 4001/4002 la du lieu migration cu, cung bi xoa.
    private static readonly int[] LegacySeededLichHenIds = [4001, 4002];
    private const string DemoLichHenPrefix = "DEMO-";

    // IdBenhNhan cua patient001 — duoc seed bang migration HasData (Module1_TestDataSeed).
    private const int IdBenhNhanDemo = 2001;
    // IdDichVu mac dinh — seed tu migration SeedDanhMuc (ID=1 luon ton tai).
    private const int IdDichVuDefault = 1;

    /// <summary>
    /// Moi lan app khoi dong:
    ///   1. Xoa LichHen demo cu (legacy IDs + DEMO- prefix) de tranh loi slot va trung key.
    ///   2. Refresh NgayLamViec cho tat ca CaLamViec seed (3001-3005).
    ///   3. Tao lai LichHen demo voi cac trang thai khac nhau cho 3 flow demo hom nay (3004).
    ///   4. Cap nhat SoSlotDaDat cho phu hop.
    /// </summary>
    private async Task SyncCaLamViecVaLichHenDemoAsync(CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(_dateTimeProvider.UtcNow);
        var now = _dateTimeProvider.UtcNow;

        // --- Buoc 1: Xoa LichHen demo cu ---
        var demoLichHen = await _db.LichHen
            .Where(lh => LegacySeededLichHenIds.Contains(lh.IdLichHen)
                         || lh.MaLichHen.StartsWith(DemoLichHenPrefix))
            .ToListAsync(cancellationToken);

        var demoLichHenIds = demoLichHen.Select(lh => lh.IdLichHen).ToList();

        var lichSuToDelete = await _db.LichSuLichHen
            .Where(ls => demoLichHenIds.Contains(ls.IdLichHen))
            .ToListAsync(cancellationToken);
        _db.LichSuLichHen.RemoveRange(lichSuToDelete);

        var hangChoToDelete = await _db.HangCho
            .Where(hc => demoLichHenIds.Contains(hc.IdLichHen))
            .ToListAsync(cancellationToken);
        _db.HangCho.RemoveRange(hangChoToDelete);

        _db.LichHen.RemoveRange(demoLichHen);

        if (demoLichHen.Count > 0)
            _logger.LogWarning("[DevFixture] Xoa {Count} LichHen demo cu.", demoLichHen.Count);

        // --- Buoc 2: Refresh CaLamViec dates ---
        var allSeeded = await _db.CaLamViec
            .Where(c => SeededCaLamViecIds.Contains(c.IdCaLamViec))
            .ToListAsync(cancellationToken);

        foreach (var ca in allSeeded)
        {
            ca.NgayLamViec = ca.IdCaLamViec switch
            {
                3004 or 3005 => today,           // hom nay
                3003         => today.AddDays(7), // tuan sau
                _            => today.AddDays(1)  // ngay mai (3001, 3002)
            };
            ca.SoSlotDaDat = 0;
        }

        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogWarning(
            "[DevFixture] Refresh {Count}/5 CaLamViec. Today={Today:yyyy-MM-dd}. Details: {Details}",
            allSeeded.Count,
            today,
            string.Join(", ", allSeeded.Select(s => $"ID={s.IdCaLamViec}→{s.NgayLamViec:MM-dd}")));

        // --- Buoc 3: Tao LichHen demo cho hom nay (3004) ---
        // Flow 2a: receptionist xac nhan
        var lichHen_ChoXacNhan = new LichHen
        {
            MaLichHen = $"{DemoLichHenPrefix}{today:yyyyMMdd}-01",
            IdBenhNhan = IdBenhNhanDemo,
            IdCaLamViec = 3004,
            IdDichVu = IdDichVuDefault,
            SoSlot = 1,
            HinhThucDat = HinhThucDat.TrucTuyen,
            TrieuChung = "Dau nguc nhe (demo - cho xac nhan)",
            TrangThai = TrangThaiLichHen.ChoXacNhan,
            NgayTao = now
        };

        // Flow 2b: receptionist check-in
        var lichHen_DaXacNhan = new LichHen
        {
            MaLichHen = $"{DemoLichHenPrefix}{today:yyyyMMdd}-02",
            IdBenhNhan = IdBenhNhanDemo,
            IdCaLamViec = 3004,
            IdDichVu = IdDichVuDefault,
            SoSlot = 2,
            HinhThucDat = HinhThucDat.TrucTuyen,
            TrieuChung = "Kho tho (demo - da xac nhan)",
            TrangThai = TrangThaiLichHen.DaXacNhan,
            NgayTao = now
        };

        // Flow 2c/d: bac si goi ke tiep va hoan thanh
        // CheckIn khong doi TrangThaiLichHen — van la DaXacNhan, chi them HangCho.ChoKham
        var lichHen_DangCho = new LichHen
        {
            MaLichHen = $"{DemoLichHenPrefix}{today:yyyyMMdd}-03",
            IdBenhNhan = IdBenhNhanDemo,
            IdCaLamViec = 3004,
            IdDichVu = IdDichVuDefault,
            SoSlot = 3,
            HinhThucDat = HinhThucDat.TrucTuyen,
            TrieuChung = "Sot cao (demo - da check-in)",
            TrangThai = TrangThaiLichHen.DaXacNhan,
            NgayTao = now
        };

        // Ngay mai (3001): dem xuat dat lich moi
        var lichHen_NgayMai = new LichHen
        {
            MaLichHen = $"{DemoLichHenPrefix}{today.AddDays(1):yyyyMMdd}-01",
            IdBenhNhan = IdBenhNhanDemo,
            IdCaLamViec = 3001,
            IdDichVu = IdDichVuDefault,
            SoSlot = 1,
            HinhThucDat = HinhThucDat.TrucTuyen,
            TrieuChung = "Kiem tra dinh ky (demo - ngay mai)",
            TrangThai = TrangThaiLichHen.ChoXacNhan,
            NgayTao = now
        };

        _db.LichHen.AddRange(lichHen_ChoXacNhan, lichHen_DaXacNhan, lichHen_DangCho, lichHen_NgayMai);
        await _db.SaveChangesAsync(cancellationToken);

        // --- Buoc 4: Tao HangCho cho LichHen DangCho ---
        _db.HangCho.Add(new HangCho
        {
            IdCaLamViec = 3004,
            IdLichHen = lichHen_DangCho.IdLichHen,
            SoThuTu = 1,
            TrangThai = TrangThaiHangCho.ChoKham,
            NgayCheckIn = now.AddMinutes(-15)
        });

        // --- Buoc 5: Cap nhat SoSlotDaDat cho dung so luong LichHen vua seed ---
        var ca3004 = allSeeded.FirstOrDefault(c => c.IdCaLamViec == 3004);
        var ca3001 = allSeeded.FirstOrDefault(c => c.IdCaLamViec == 3001);
        if (ca3004 != null) ca3004.SoSlotDaDat = 3;
        if (ca3001 != null) ca3001.SoSlotDaDat = 1;

        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogWarning(
            "[DevFixture] Da seed 4 LichHen demo: 3x ca {Ca1} (hom nay), 1x ca {Ca2} (ngay mai).",
            3004, 3001);
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
