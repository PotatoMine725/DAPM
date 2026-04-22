using System.Net.Http.Headers;
using System.Net.Http.Json;
using ClinicBooking.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicBooking.IntegrationTests.Common;

/// <summary>
/// Base class cho integration test su dung <see cref="TestWebAppFactory"/>.
/// Moi test class ke thua tu day de dung chung HttpClient va DB scope.
/// </summary>
public abstract class IntegrationTestBase : IClassFixture<TestWebAppFactory>, IAsyncLifetime
{
    protected readonly TestWebAppFactory Factory;
    protected readonly HttpClient Client;

    protected IntegrationTestBase(TestWebAppFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    /// <summary>
    /// Lay JWT token bang cach dang nhap voi tai khoan cho truoc.
    /// Dung trong test can xac thuc.
    /// </summary>
    protected async Task<string> LayTokenAsync(string tenDangNhap, string matKhau)
    {
        var response = await Client.PostAsJsonAsync("/api/auth/dang-nhap", new
        {
            tenDangNhap,
            matKhau
        });

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<DangNhapTestResult>();
        return result?.AccessToken ?? throw new InvalidOperationException("Khong nhan duoc access token.");
    }

    /// <summary>
    /// Set Authorization header cho HttpClient.
    /// </summary>
    protected void DatToken(string token)
    {
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    /// <summary>
    /// Xoa token khoi header.
    /// </summary>
    protected void XoaToken()
    {
        Client.DefaultRequestHeaders.Authorization = null;
    }

    /// <summary>Reset DB truoc moi test.</summary>
    public virtual async Task InitializeAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.EnsureCreatedAsync();
    }

    public virtual Task DisposeAsync() => Task.CompletedTask;

    // DTO noi bo de deserialize ket qua dang nhap
    private sealed record DangNhapTestResult(string AccessToken, string RefreshToken);
}
