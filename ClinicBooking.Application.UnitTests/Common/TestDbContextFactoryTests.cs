using FluentAssertions;

namespace ClinicBooking.Application.UnitTests.Common;

/// <summary>
/// Smoke test dam bao <see cref="TestDbContextFactory"/> tao duoc AppDbContext tren SQLite thanh cong.
/// </summary>
public class TestDbContextFactoryTests : IDisposable
{
    private readonly TestDbContextFactory _factory = new();

    [Fact]
    public void TaoContext_CoTheTruyCapDbSet_LichHen()
    {
        // Arrange & Act
        using var context = _factory.CreateContext();

        // Assert — DbSet co the duoc truy cap ma khong throw
        var count = context.LichHen.Count();
        count.Should().Be(0);
    }

    [Fact]
    public void TaoContext_CoTheTruyCapDbSet_HangCho()
    {
        using var context = _factory.CreateContext();

        var count = context.HangCho.Count();
        count.Should().Be(0);
    }

    public void Dispose()
    {
        _factory.Dispose();
        GC.SuppressFinalize(this);
    }
}
