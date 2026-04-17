using ClinicBooking.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.UnitTests.Common;

/// <summary>
/// Factory tao <see cref="AppDbContext"/> tren SQLite :memory: de unit test chay nhanh
/// ma van co unique index + FK constraint thuc su (khac InMemoryDatabase khong co).
/// <br/>
/// <b>Luu y</b>: SQLite khong ho tro <c>rowversion</c> nhu SQL Server.
/// <c>ExecuteUpdateAsync</c> cung co semantics khac. Concurrency test de Wave 4 (Testcontainers MsSql).
/// </summary>
public class TestDbContextFactory : IDisposable
{
    private readonly SqliteConnection _connection;

    public TestDbContextFactory()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
    }

    /// <summary>
    /// Tao mot <see cref="AppDbContext"/> chia se cung 1 SQLite connection.
    /// Goi <c>Database.EnsureCreated()</c> de tao schema.
    /// </summary>
    public AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        var context = new AppDbContext(options);
        context.Database.EnsureCreated();

        return context;
    }

    public void Dispose()
    {
        _connection.Dispose();
        GC.SuppressFinalize(this);
    }
}
