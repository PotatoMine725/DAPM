using System.Net.Http.Headers;
using System.Net.Http.Json;
using ClinicBooking.Api.Contracts.Auth;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Features.Auth.Dtos;
using ClinicBooking.Domain.Entities;
using ClinicBooking.Domain.Enums;
using ClinicBooking.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace ClinicBooking.Integration.Tests;

public sealed class ClinicBookingApiFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;
    private readonly string _databaseName;

    public DateOnly NgayLamViecHomSau { get; }
    public DateOnly NgayLamViecTiepTheo { get; }

    public ClinicBookingApiFactory()
    {
        var homNay = DateTime.UtcNow.Date;
        NgayLamViecHomSau = DateOnly.FromDateTime(homNay.AddDays(1));
        NgayLamViecTiepTheo = DateOnly.FromDateTime(homNay.AddDays(7));

        _databaseName = $"ClinicBooking_Integration_{Guid.NewGuid():N}";
        _connectionString = $"Server=(localdb)\\MSSQLLocalDB;Database={_databaseName};Integrated Security=True;MultipleActiveResultSets=True;TrustServerCertificate=True";

        using var context = CreateSchemaContext();
        context.Database.Migrate();

        var caCanDoi = new[] { 3001, 3002, 3003 };
        var cacCaLamViec = context.CaLamViec
            .Where(x => caCanDoi.Contains(x.IdCaLamViec))
            .ToList();

        foreach (var caLamViec in cacCaLamViec)
        {
            caLamViec.NgayLamViec = caLamViec.IdCaLamViec == 3003 ? NgayLamViecTiepTheo : NgayLamViecHomSau;
        }

        context.SaveChanges();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            var values = new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = _connectionString,
                ["Admin:MatKhauMacDinh"] = "Demo@123456",
                ["Admin:DevFixture:Enabled"] = "true",
                ["Admin:DevFixture:MatKhauChung"] = "Demo@123456",
                ["Admin:DevFixture:BenhNhan:TenDangNhap"] = "patient001",
                ["Admin:DevFixture:BenhNhan:Email"] = "patient@test.vn",
                ["Admin:DevFixture:BenhNhan:SoDienThoai"] = "0912345678",
                ["Admin:DevFixture:BenhNhan:HoTen"] = "Tran Thi B",
                ["Admin:DevFixture:BacSi:TenDangNhap"] = "doctor001",
                ["Admin:DevFixture:BacSi:Email"] = "doctor@test.vn",
                ["Admin:DevFixture:BacSi:SoDienThoai"] = "0987654321",
                ["Admin:DevFixture:BacSi:HoTen"] = "Dr. Nguyen Van A",
                ["Admin:DevFixture:LeTan:TenDangNhap"] = "receptionist001",
                ["Admin:DevFixture:LeTan:Email"] = "receptionist@test.vn",
                ["Admin:DevFixture:LeTan:SoDienThoai"] = "0911111111",
                ["Admin:DevFixture:LeTan:HoTen"] = "Le Tan Demo",
                ["Admin:DevFixture:Admin:TenDangNhap"] = "admin001",
                ["Admin:DevFixture:Admin:Email"] = "admin@test.vn",
                ["Admin:DevFixture:Admin:SoDienThoai"] = "0988888888",
                ["Admin:DevFixture:Admin:HoTen"] = "Admin Demo",
            };

            configuration.AddInMemoryCollection(values);
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IHostedService>();
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        try
        {
            using var context = CreateSchemaContext();
            context.Database.EnsureDeleted();
        }
        catch (Exception)
        {
        }
    }

    /// <summary>
    /// Dang nhap va tra ve HttpClient da gan Authorization Bearer header.
    /// </summary>
    public async Task<HttpClient> LoginAsync(string tenDangNhap, string matKhau, CancellationToken cancellationToken = default)
    {
        var baseOptions = new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost") };
        using var plainClient = CreateClient(baseOptions);

        var response = await plainClient.PostAsJsonAsync(
            "/api/auth/dang-nhap",
            new DangNhapRequest(tenDangNhap, matKhau),
            cancellationToken);
        response.EnsureSuccessStatusCode();

        var xacThuc = await response.Content.ReadFromJsonAsync<XacThucResponse>(cancellationToken: cancellationToken);

        var client = CreateClient(baseOptions);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", xacThuc!.AccessToken);
        return client;
    }

    /// <summary>
    /// Tao tai khoan BenhNhan moi (idempotent) va tra ve client da dang nhap.
    /// Dung de tao "benh_nhan thu hai" cho cac test kiem tra ownership.
    /// </summary>
    public async Task<HttpClient> TaoVaDangNhapBenhNhanMoiAsync(
        string tenDangNhap,
        string matKhau,
        CancellationToken cancellationToken = default)
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var dateTimeProvider = scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();

        if (!await db.TaiKhoan.AnyAsync(x => x.TenDangNhap == tenDangNhap, cancellationToken))
        {
            var taiKhoan = new TaiKhoan
            {
                TenDangNhap = tenDangNhap,
                MatKhau = hasher.HashPassword(matKhau),
                VaiTro = VaiTro.BenhNhan,
                TrangThai = true,
                NgayTao = dateTimeProvider.UtcNow
            };
            db.TaiKhoan.Add(taiKhoan);
            await db.SaveChangesAsync(cancellationToken);

            db.BenhNhan.Add(new BenhNhan
            {
                IdTaiKhoan = taiKhoan.IdTaiKhoan,
                HoTen = tenDangNhap,
                NgayTao = dateTimeProvider.UtcNow
            });
            await db.SaveChangesAsync(cancellationToken);
        }

        return await LoginAsync(tenDangNhap, matKhau, cancellationToken);
    }

    private AppDbContext CreateSchemaContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(_connectionString)
            .Options;

        return new AppDbContext(options);
    }
}
