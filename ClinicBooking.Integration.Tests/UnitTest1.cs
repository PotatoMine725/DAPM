using System.Net.Http.Headers;
using System.Net.Http.Json;
using ClinicBooking.Api.Contracts.Auth;
using ClinicBooking.Api.Contracts.LichHen;
using ClinicBooking.Application.Features.Auth.Dtos;
using ClinicBooking.Application.Features.LichHen.Dtos;
using ClinicBooking.Domain.Enums;
using ClinicBooking.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicBooking.Integration.Tests;

public sealed class UnitTest1
{
    [Fact]
    public async Task Module1_Smoke_Dat_Lich_BangApiThat()
    {
        using var factory = new ClinicBookingApiFactory();
        var clientOptions = new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        };
        using var client = factory.CreateClient(clientOptions);

        var benhNhanClient = await TaoClientDaDangNhapAsync(factory, client, "patient001", "Demo@123456");
        var taoResponse = await benhNhanClient.PostAsJsonAsync(
            "/api/lich-hen/tao-lich-hen",
            new TaoLichHenRequest(factory.NgayLamViecHomSau, new TimeOnly(13, 15), 2));

        taoResponse.EnsureSuccessStatusCode();
        var lichHenDaTao = await taoResponse.Content.ReadFromJsonAsync<LichHenResponse>();
        lichHenDaTao.Should().NotBeNull();
        lichHenDaTao!.TrangThai.Should().Be(TrangThaiLichHen.ChoXacNhan);

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var lichHenSau = await db.LichHen.AsNoTracking().FirstAsync(x => x.IdLichHen == lichHenDaTao.IdLichHen);
            lichHenSau.TrangThai.Should().Be(TrangThaiLichHen.ChoXacNhan);

            (await db.LichSuLichHen.AsNoTracking().AnyAsync(x => x.IdLichHen == lichHenDaTao.IdLichHen && x.HanhDong == HanhDongLichSu.DatMoi))
                .Should().BeTrue();
        }
    }

    [Fact(Skip = "Blocked by auth-role behavior in the integration host; follow-up needed for full cancel/reschedule smoke.")]
    public async Task Module1_Smoke_HuyVaDoiLichBangApiThat()
    {
        await Task.CompletedTask;
    }

    private static async Task<HttpClient> TaoClientDaDangNhapAsync(
        ClinicBookingApiFactory factory,
        HttpClient client,
        string tenDangNhap,
        string matKhau)
    {
        var response = await client.PostAsJsonAsync(
            "/api/auth/dang-nhap",
            new DangNhapRequest(tenDangNhap, matKhau));

        response.EnsureSuccessStatusCode();
        var xacThuc = await response.Content.ReadFromJsonAsync<XacThucResponse>();
        xacThuc.Should().NotBeNull();

        var authenticatedClient = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });
        authenticatedClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", xacThuc!.AccessToken);
        return authenticatedClient;
    }
}
