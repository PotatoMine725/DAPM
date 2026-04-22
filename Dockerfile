# ============================================================
# Stage 1 — Build
# ============================================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files truoc de tan dung Docker layer cache
COPY ClinicBooking.Domain/ClinicBooking.Domain.csproj            ClinicBooking.Domain/
COPY ClinicBooking.Application/ClinicBooking.Application.csproj  ClinicBooking.Application/
COPY ClinicBooking.Infrastructure/ClinicBooking.Infrastructure.csproj ClinicBooking.Infrastructure/
COPY ClinicBooking.Api/ClinicBooking.Api.csproj                  ClinicBooking.Api/

# Restore NuGet packages
RUN dotnet restore ClinicBooking.Api/ClinicBooking.Api.csproj

# Copy toan bo source code
COPY . .

# Build va publish (Release mode)
RUN dotnet publish ClinicBooking.Api/ClinicBooking.Api.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish

# ============================================================
# Stage 2 — Runtime
# ============================================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Tao user non-root de chay app (security best practice)
RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser

COPY --from=build /app/publish .

# Chuyen quyen cho user non-root
RUN chown -R appuser:appgroup /app
USER appuser

# Port expose (ASP.NET Core mac dinh 8080 trong container)
EXPOSE 8080
EXPOSE 8081

# Environment variables mac dinh (override qua docker-compose hoac env)
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "ClinicBooking.Api.dll"]
