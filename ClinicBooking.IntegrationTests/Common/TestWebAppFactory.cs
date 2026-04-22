using ClinicBooking.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ClinicBooking.IntegrationTests.Common;

/// <summary>
/// WebApplicationFactory dung SQLite in-memory de integration test chay nhanh
/// ma khong can SQL Server that (phu hop CI/CD).
/// Su dung <see cref="TestcontainersSqlServerFactory"/> neu can SQL Server that.
/// </summary>
public class TestWebAppFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Thay SQL Server DbContext bang SQLite in-memory
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<AppDbContext>();

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlite("DataSource=:memory:;Cache=Shared");
                // Tat warning rowversion vi SQLite khong ho tro
                options.ConfigureWarnings(w =>
                    w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
            });

            // Dam bao DB duoc tao truoc khi test chay
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
        });
    }
}
