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
    private bool _schemaReady;

    public TestDbContextFactory()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
    }

    /// <summary>
    /// Tao mot <see cref="AppDbContext"/> chia se cung 1 SQLite connection.
    /// Schema chi tao mot lan moi factory instance — cac CreateContext sau tai su dung cung schema/du lieu
    /// de cac test multi-context (concurrency, factory behavior) nhin thay du lieu chung.
    /// </summary>
    public AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        var context = new AppDbContext(options);
        if (_schemaReady)
        {
            return context;
        }

        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        // EnsureCreated() applies HasData from model snapshot (including module test fixtures).
        // Xoa toan bo du lieu nghiep vu de moi test bat dau voi DB sach, tranh xung dot unique key.
        var deleteStatements = new[]
        {
            "DELETE FROM LichSuLichHen",
            "DELETE FROM GiuCho",
            "DELETE FROM HangCho",
            "DELETE FROM HoSoKham",
            "DELETE FROM ToaThuoc",
            "DELETE FROM LichHen",
            "DELETE FROM DonNghiPhep",
            "DELETE FROM CaLamViec",
            "DELETE FROM LichNoiTru",
            "DELETE FROM BenhNhan",
            "DELETE FROM BacSi",
            "DELETE FROM LeTan",
            "DELETE FROM RefreshToken",
            "DELETE FROM OtpLog",
            "DELETE FROM ThongBao",
            "DELETE FROM TaiKhoan WHERE IdTaiKhoan <> 1",
            "DELETE FROM ChuyenKhoa WHERE IdChuyenKhoa > 100",
            "DELETE FROM DichVu WHERE IdDichVu > 100",
            "DELETE FROM Phong WHERE IdPhong > 100",
            "DELETE FROM DinhNghiaCa WHERE IdDinhNghiaCa > 100",
            "DELETE FROM Thuoc WHERE IdThuoc > 100"
        };
        foreach (var sql in deleteStatements)
        {
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }

        _schemaReady = true;
        return context;
    }

    public void Dispose()
    {
        _connection.Dispose();
        GC.SuppressFinalize(this);
    }
}
