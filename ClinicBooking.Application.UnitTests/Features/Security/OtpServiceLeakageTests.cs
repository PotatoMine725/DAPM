using ClinicBooking.Application.Abstractions.Notifications;
using ClinicBooking.Application.Common.Options;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Entities;
using ClinicBooking.Domain.Enums;
using ClinicBooking.Infrastructure.Services.Security;
using FluentAssertions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace ClinicBooking.Application.UnitTests.Features.Security;

/// <summary>
/// Bao ve dieu kien: ma OTP KHONG duoc ghi ra log khi chay o moi truong Production.
/// Chi cho phep o Development (de demo / dev fallback).
/// </summary>
public class OtpServiceLeakageTests : IDisposable
{
    private readonly TestDbContextFactory _factory = new();

    [Fact]
    public async Task TaoVaGuiOtp_KhongCoEmail_TrongProduction_KhongLogMaOtp()
    {
        var (service, logger, otp) = await SetupAndCallAsync("Production", emailService: BatchFakeEmailService(succeed: true));

        var allMessages = string.Join("\n", logger.Messages);
        allMessages.Should().Contain("khong co email hop le");
        allMessages.Should().NotContain(otp, "ma OTP khong duoc leak ra log o moi truong Production");
        allMessages.Should().NotContain("OTP-DEV", "tag dev-only khong duoc xuat hien o Production");
    }

    [Fact]
    public async Task TaoVaGuiOtp_KhongCoEmail_TrongDevelopment_LogKemMaOtp()
    {
        var (service, logger, otp) = await SetupAndCallAsync("Development", emailService: BatchFakeEmailService(succeed: true));

        var allMessages = string.Join("\n", logger.Messages);
        allMessages.Should().Contain("OTP-DEV");
        allMessages.Should().Contain(otp, "Development phai log ma OTP de demo");
    }

    [Fact]
    public async Task TaoVaGuiOtp_EmailServiceThrow_TrongProduction_KhongLogMaOtp()
    {
        var (service, logger, otp) = await SetupAndCallAsync(
            "Production",
            emailService: BatchFakeEmailService(succeed: false),
            emailDich: "user@example.com");

        var allMessages = string.Join("\n", logger.Messages);
        allMessages.Should().Contain("Gui email that bai");
        allMessages.Should().NotContain(otp);
        allMessages.Should().NotContain("OTP-DEV");
    }

    // --- helpers ---

    private async Task<(OtpService Service, CapturingLogger<OtpService> Logger, string Otp)> SetupAndCallAsync(
        string envName,
        IEmailService emailService,
        string? emailDich = null)
    {
        using var ctx = _factory.CreateContext();
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var tk = new TaiKhoan
        {
            TenDangNhap = $"otp_leak_{suffix}",
            Email = emailDich ?? $"walkin_{suffix}@local.invalid",
            SoDienThoai = $"09{suffix[..8]}",
            MatKhau = "x",
            VaiTro = VaiTro.BenhNhan,
            TrangThai = true,
            NgayTao = DateTime.UtcNow
        };
        ctx.TaiKhoan.Add(tk);
        await ctx.SaveChangesAsync();

        var logger = new CapturingLogger<OtpService>();
        var env = Substitute.For<IHostEnvironment>();
        env.EnvironmentName.Returns(envName);

        var options = Options.Create(new OtpOptions { ThoiHanPhut = 5, SoLanNhapSaiToiDa = 3 });

        var service = new OtpService(ctx, emailService, logger, options, env);

        var otp = await service.TaoVaGuiOtpDatLichAsync(tk.IdTaiKhoan, tk.SoDienThoai);
        return (service, logger, otp);
    }

    private static IEmailService BatchFakeEmailService(bool succeed)
    {
        var fake = Substitute.For<IEmailService>();
        if (!succeed)
        {
            fake.GuiEmailAsync(
                    Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(),
                    Arg.Any<bool>(), Arg.Any<CancellationToken>())
                .Returns<Task>(_ => throw new InvalidOperationException("smtp down"));
        }
        return fake;
    }

    public void Dispose() => _factory.Dispose();

    private sealed class CapturingLogger<T> : ILogger<T>
    {
        public List<string> Messages { get; } = new();

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            Messages.Add(formatter(state, exception));
        }
    }
}
