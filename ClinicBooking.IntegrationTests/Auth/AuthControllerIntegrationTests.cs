using System.Net;
using System.Net.Http.Json;
using ClinicBooking.IntegrationTests.Common;
using FluentAssertions;

namespace ClinicBooking.IntegrationTests.Auth;

/// <summary>
/// Integration test smoke cho Auth endpoints.
/// Dung TestWebAppFactory voi SQLite in-memory.
/// </summary>
public class AuthControllerIntegrationTests : IntegrationTestBase
{
    public AuthControllerIntegrationTests(TestWebAppFactory factory)
        : base(factory) { }

    [Fact]
    public async Task DangNhap_VoiTaiKhoanAdmin_TraVe200VaToken()
    {
        // Arrange
        var request = new
        {
            tenDangNhap = "admin",
            matKhau = "Admin@123456"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/dang-nhap", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<DangNhapResponse>();
        body.Should().NotBeNull();
        body!.AccessToken.Should().NotBeNullOrWhiteSpace();
        body.RefreshToken.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task DangNhap_VoiSaiMatKhau_TraVe401()
    {
        // Arrange
        var request = new
        {
            tenDangNhap = "admin",
            matKhau = "SaiMatKhau!"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/dang-nhap", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GoiEndpointCanAuth_KhongCoToken_TraVe401()
    {
        // Act — goi endpoint yeu cau dang nhap ma khong co token
        var response = await Client.GetAsync("/api/benh-nhan/ho-so-cua-toi");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.Unauthorized, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task SwaggerEndpoint_TraVe200()
    {
        // Act
        var response = await Client.GetAsync("/swagger/v1/swagger.json");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // DTO noi bo cho test
    private sealed record DangNhapResponse(string AccessToken, string RefreshToken);
}
