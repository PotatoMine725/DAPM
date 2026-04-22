using ClinicBooking.Application.Abstractions.Notifications;
using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Abstractions.Scheduling;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Options;
using ClinicBooking.Application.Common.Services;
using ClinicBooking.Infrastructure.Identity;
using ClinicBooking.Infrastructure.Persistence;
using ClinicBooking.Infrastructure.Security;
using ClinicBooking.Infrastructure.Services;
using ClinicBooking.Infrastructure.Services.Notifications;
using ClinicBooking.Infrastructure.Services.Scheduling;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicBooking.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

        services.Configure<JwtSettings>(
            configuration.GetSection(JwtSettings.SectionName));
        services.Configure<AdminSeederSettings>(
            configuration.GetSection(AdminSeederSettings.SectionName));

        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, TokenService>();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddScoped<DatabaseSeeder>();

        // --- Module 1: Dat lich hen & Hang cho ---
        services.Configure<LichHenOptions>(
            configuration.GetSection(LichHenOptions.SectionName));

        services.AddScoped<ICaLamViecQueryService, CaLamViecQueryService>();

        // TODO: Thay NotificationServiceStub bang impl Module 4 khi code duoc day len
        services.AddScoped<INotificationService, NotificationServiceStub>();

        services.AddScoped<IMaLichHenGenerator, MaLichHenGenerator>();

        return services;
    }
}
