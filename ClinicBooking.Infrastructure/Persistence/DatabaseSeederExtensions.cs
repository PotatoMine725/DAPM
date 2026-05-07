using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ClinicBooking.Infrastructure.Persistence;

public static class DatabaseSeederExtensions
{
    /// <summary>
    /// Ap dung pending EF migrations (chi Development) roi chay DatabaseSeeder.
    /// Goi sau khi Build() va truoc khi Run().
    /// </summary>
    public static async Task SeedDatabaseAsync(
        this WebApplication app,
        CancellationToken cancellationToken = default)
    {
        await using var scope = app.Services.CreateAsyncScope();

        if (app.Environment.IsDevelopment())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await db.Database.MigrateAsync(cancellationToken);
        }

        var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedAsync(cancellationToken);
    }
}
