using System.Net;
using System.Net.Http.Json;
using ClinicBooking.Api.Contracts.LichHen;
using ClinicBooking.Application.Features.LichHen.Dtos;
using FluentAssertions;

namespace ClinicBooking.Integration.Tests;

/// <summary>
/// Integration tests cho POST /api/lich-hen/{id}/huy.
/// Su dung IClassFixture nen tat ca tests trong class nay dung chung mot LocalDB.
/// </summary>
public sealed class HuyLichHenIntegrationTests : IClassFixture<ClinicBookingApiFactory>
{
    private readonly ClinicBookingApiFactory _factory;

    public HuyLichHenIntegrationTests(ClinicBookingApiFactory factory)
    {
        _factory = factory;
    }

    // Helper: tao lich hen cho patient001, tra ve IdLichHen.
    // Truyen ngayLamViec + gioMongMuon de chon dung CaLamViec can dung, tranh slot collision giua cac tests.
    // CaLamViec 3001 co LichHen seed (SoSlot=1,2) nen KHONG dung. Chon 3002 hoac 3003.
    private async Task<int> TaoLichHenAsync(DateOnly ngayLamViec, TimeOnly gioMongMuon, CancellationToken ct = default)
    {
        var client = await _factory.LoginAsync("patient001", "Demo@123456", ct);
        var response = await client.PostAsJsonAsync(
            "/api/lich-hen/tao-lich-hen",
            new TaoLichHenRequest(ngayLamViec, gioMongMuon, 2),
            ct);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<LichHenResponse>(cancellationToken: ct);
        return result!.IdLichHen;
    }

    [Fact]
    public async Task Huy_BenhNhanChuSoHuu_Tra200()
    {
        // CaLamViec 3002 (NgayLamViecHomSau, 13:00–17:00) — khong co LichHen seed
        var idLichHen = await TaoLichHenAsync(_factory.NgayLamViecHomSau, new TimeOnly(13, 15));
        var client = await _factory.LoginAsync("patient001", "Demo@123456");

        var response = await client.PostAsJsonAsync(
            $"/api/lich-hen/{idLichHen}/huy",
            new HuyLichHenRequest("Huy de kiem tra — integration test"));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Huy_BenhNhanKhongSoHuu_Tra403()
    {
        // Dung CaLamViec 3003 (NgayLamViecTiepTheo, 07:00–12:00) — khong co LichHen seed, khac han 3002
        var idLichHen = await TaoLichHenAsync(_factory.NgayLamViecTiepTheo, new TimeOnly(8, 0));

        // patient002 (tai khoan khac) co gang huy lich cua patient001
        var patient2Client = await _factory.TaoVaDangNhapBenhNhanMoiAsync(
            "patient_huy_other",
            "Demo@123456");

        var response = await patient2Client.PostAsJsonAsync(
            $"/api/lich-hen/{idLichHen}/huy",
            new HuyLichHenRequest("Thu huy lich cua nguoi khac"));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Huy_LichHenKhongTonTai_Tra404()
    {
        var client = await _factory.LoginAsync("patient001", "Demo@123456");

        var response = await client.PostAsJsonAsync(
            "/api/lich-hen/999999/huy",
            new HuyLichHenRequest("Lich hen khong ton tai"));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
