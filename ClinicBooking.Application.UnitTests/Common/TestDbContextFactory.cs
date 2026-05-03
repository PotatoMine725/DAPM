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
    private bool _initialized;

    public TestDbContextFactory()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
    }

    /// <summary>
    /// Tao mot <see cref="AppDbContext"/> chia se cung 1 SQLite connection.
    /// Goi <c>Database.EnsureCreated()</c> de tao schema.
    /// <br/>
    /// Lan goi dau tien se xoa du lieu seed tu migration Module1_TestDataSeed
    /// (IDs >= 2000) de tranh xung dot unique constraint voi du lieu test.
    /// </summary>
    public AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        var context = new AppDbContext(options);
        context.Database.EnsureCreated();

        if (!_initialized)
        {
            _initialized = true;
            // EnsureCreated() applies HasData from model snapshot (Module1_TestDataSeed).
            // Xoa de unit test bat dau voi DB trong, khong bi anh huong boi du lieu dev fixture.
            // Thu tu xoa theo dependency: con truoc cha sau (FK cascade khong tu dong).
            // SQLite ExecuteNonQuery chi chay 1 statement moi lan -> phai tach rieng.
            var deleteStatements = new[]
            {
                // Cap la (phu thuoc nhieu nhat) xoa truoc
                "DELETE FROM LichSuLichHen WHERE IdLichSu >= 6000",  // -> LichHen
                "DELETE FROM GiuCho WHERE IdGiuCho >= 1",            // -> CaLamViec (khong co nguong cu the)
                "DELETE FROM HangCho WHERE IdHangCho >= 5000",       // -> LichHen, CaLamViec
                "DELETE FROM LichHen WHERE IdLichHen >= 4000",       // -> BenhNhan, CaLamViec
                "DELETE FROM CaLamViec WHERE IdCaLamViec >= 3000",   // -> BacSi, Phong, ChuyenKhoa
                "DELETE FROM BenhNhan WHERE IdBenhNhan >= 2000",     // -> TaiKhoan
                "DELETE FROM BacSi WHERE IdBacSi >= 2000",           // -> TaiKhoan, ChuyenKhoa
                "DELETE FROM LeTan WHERE IdLeTan >= 2000",           // -> TaiKhoan
                "DELETE FROM TaiKhoan WHERE IdTaiKhoan >= 2000",
            };
            foreach (var sql in deleteStatements)
            {
                using var cmd = _connection.CreateCommand();
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
        }

        return context;
    }

    public void Dispose()
    {
        _connection.Dispose();
        GC.SuppressFinalize(this);
    }
}
