using System.Net;
using System.Net.Http.Json;
using ClinicBooking.Api.Contracts.LichHen;
using ClinicBooking.Application.Features.LichHen.Dtos;
using ClinicBooking.Domain.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ClinicBooking.Integration.Tests;

/// <summary>
/// Integration tests cho POST /api/lich-hen/tao-lich-hen.
/// Su dung IClassFixture nen tat ca tests trong class nay dung chung mot LocalDB.
/// </summary>
public sealed class TaoLichHenIntegrationTests : IClassFixture<ClinicBookingApiFactory>
{
    private readonly ClinicBookingApiFactory _factory;

    public TaoLichHenIntegrationTests(ClinicBookingApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Tao_BenhNhan_DatLichHopLe_Tra201()
    {
        var client = await _factory.LoginAsync("patient001", "Demo@123456");

        var response = await client.PostAsJsonAsync(
            "/api/lich-hen/tao-lich-hen",
            // TimeOnly(13,15) roi vao khung 13:00-17:00 => CaLamViec 3002 (NgayLamViecHomSau)
            new TaoLichHenRequest(_factory.NgayLamViecHomSau, new TimeOnly(13, 15), 2));

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<LichHenResponse>();
        result.Should().NotBeNull();
        result!.TrangThai.Should().Be(TrangThaiLichHen.ChoXacNhan);
        result.NgayLamViec.Should().Be(_factory.NgayLamViecHomSau);
        result.IdLichHen.Should().BeGreaterThan(0);
        result.MaLichHen.Should().StartWith("LH-");
    }

    [Fact]
    public async Task Tao_KhongCoToken_Tra401()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });

        var response = await client.PostAsJsonAsync(
            "/api/lich-hen/tao-lich-hen",
            new TaoLichHenRequest(_factory.NgayLamViecHomSau, new TimeOnly(13, 15), 2));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Tao_RoleBacSi_Tra403()
    {
        var client = await _factory.LoginAsync("doctor001", "Demo@123456");

        var response = await client.PostAsJsonAsync(
            "/api/lich-hen/tao-lich-hen",
            new TaoLichHenRequest(_factory.NgayLamViecHomSau, new TimeOnly(13, 15), 2));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Tao_InputThieu_Tra400()
    {
        var client = await _factory.LoginAsync("patient001", "Demo@123456");

        // IdDichVu = 0 va NgayLamViec = default (0001-01-01) vi pham FluentValidation
        var response = await client.PostAsJsonAsync(
            "/api/lich-hen/tao-lich-hen",
            new TaoLichHenRequest(DateOnly.MinValue, TimeOnly.MinValue, 0));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
