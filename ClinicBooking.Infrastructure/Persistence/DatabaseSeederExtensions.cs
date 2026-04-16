using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicBooking.Infrastructure.Persistence;

public static class DatabaseSeederExtensions
{
    /// <summary>
    /// Chay <see cref="DatabaseSeeder"/> trong mot scope rieng khi app khoi dong.
    /// Goi sau khi Build() va truoc khi Run().
    /// </summary>
    public static async Task SeedDatabaseAsync(
        this WebApplication app,
        CancellationToken cancellationToken = default)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedAsync(cancellationToken);
    }
}
