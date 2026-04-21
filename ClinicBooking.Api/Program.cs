using ClinicBooking.Api.Extensions;
using ClinicBooking.Api.Middleware;
using ClinicBooking.Application;
using ClinicBooking.Infrastructure;
using ClinicBooking.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Extensions.TryAdd("traceId", context.HttpContext.TraceIdentifier);
        context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.Connection.Id);
    };
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ClinicBooking API",
        Version = "v1"
    });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Nhap JWT access token duoi dang: Bearer {token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });
});

var swaggerRoutePrefix = builder.Configuration["Swagger:RoutePrefix"];
var swaggerRootPath = string.IsNullOrWhiteSpace(swaggerRoutePrefix) || swaggerRoutePrefix == "/"
    ? "/swagger"
    : $"/{swaggerRoutePrefix.Trim('/')}";

// MediatR va cac handler yeu cau Application layer tham chieu den Infrastructure
// cho DbContext, nen dam bao AddInfrastructure chay truoc AddApplication.

var app = builder.Build();

// Seed du lieu can "sua sau migration" (vi du: thay hash admin gia lap bang hash that).
// Goi truoc khi middleware xu ly request — yeu cau database da duoc migrate.
await app.SeedDatabaseAsync();

app.UseExceptionHandler();
app.UseStatusCodePages();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "ClinicBooking API v1");
});

app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () => Results.Redirect(swaggerRootPath));

app.Run();
