using System.Net;
using System.Net.Http.Json;
using ClinicBooking.Api.Contracts.LichHen;
using ClinicBooking.Application.Features.LichHen.Dtos;
using ClinicBooking.Domain.Enums;
using ClinicBooking.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicBooking.Integration.Tests;

/// <summary>
/// Integration tests cho POST /api/lich-hen/{id}/doi-lich.
/// Su dung IClassFixture nen tat ca tests trong class nay dung chung mot LocalDB.
/// </summary>
public sealed class DoiLichHenIntegrationTests : IClassFixture<ClinicBookingApiFactory>
{
    private readonly ClinicBookingApiFactory _factory;

    public DoiLichHenIntegrationTests(ClinicBookingApiFactory factory)
    {
        _factory = factory;
    }

    // Helper: patient001 tao lich hen, tra ve IdLichHen.
    // Su dung CaLamViec 3002 (NgayLamViecHomSau, 13:00-17:00) de doi sang 3003 con trong.
    private async Task<int> TaoLichHenTren3002Async(CancellationToken ct = default)
    {
        var client = await _factory.LoginAsync("patient001", "Demo@123456", ct);
        var response = await client.PostAsJsonAsync(
            "/api/lich-hen/tao-lich-hen",
            new TaoLichHenRequest(_factory.NgayLamViecHomSau, new TimeOnly(13, 15), 2),
            ct);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<LichHenResponse>(cancellationToken: ct);
        return result!.IdLichHen;
    }

    [Fact]
    public async Task Doi_LeTanDoiHopLe_Tra200_CoIdLichHenMoi()
    {
        var idLichHenCu = await TaoLichHenTren3002Async();
        var letanClient = await _factory.LoginAsync("receptionist001", "Demo@123456");

        // Doi sang CaLamViec 3003 (NgayLamViecTiepTheo, 07:00-12:00)
        var response = await letanClient.PostAsJsonAsync(
            $"/api/lich-hen/{idLichHenCu}/doi-lich",
            new DoiLichHenRequest(3003, LyDo: "Doi sang ca khac — integration test"));

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<LichHenResponse>();
        result.Should().NotBeNull();
        result!.IdCaLamViec.Should().Be(3003);
        result.TrangThai.Should().Be(TrangThaiLichHen.ChoXacNhan);
        result.NgayLamViec.Should().Be(_factory.NgayLamViecTiepTheo);
        // IdLichHen moi phai khac lich hen cu (voi doi lich, mot lich moi duoc tao)
        result.IdLichHen.Should().NotBe(idLichHenCu);
    }

    [Fact]
    public async Task Doi_BenhNhanKhongSoHuu_Tra403()
    {
        // patient001 tao lich hen
        var idLichHen = await TaoLichHenTren3002Async();

        // patient002 co gang doi lich cua patient001
        var patient2Client = await _factory.TaoVaDangNhapBenhNhanMoiAsync(
            "patient_doi_other",
            "Demo@123456");

        var response = await patient2Client.PostAsJsonAsync(
            $"/api/lich-hen/{idLichHen}/doi-lich",
            new DoiLichHenRequest(3003, LyDo: "Thu doi lich cua nguoi khac"));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Doi_LichDaHoanThanh_Tra409()
    {
        // Tao lich hen roi truc tiep set trang thai = HoanThanh trong DB
        var idLichHen = await TaoLichHenTren3002Async();

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await db.LichHen
                .Where(x => x.IdLichHen == idLichHen)
                .ExecuteUpdateAsync(s => s.SetProperty(x => x.TrangThai, TrangThaiLichHen.HoanThanh));
        }

        // Bat ky nguoi dung hop le co quyen doi lich deu phai nhan 409
        var letanClient = await _factory.LoginAsync("receptionist001", "Demo@123456");
        var response = await letanClient.PostAsJsonAsync(
            $"/api/lich-hen/{idLichHen}/doi-lich",
            new DoiLichHenRequest(3003, LyDo: "Doi lich da hoan thanh"));

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}
