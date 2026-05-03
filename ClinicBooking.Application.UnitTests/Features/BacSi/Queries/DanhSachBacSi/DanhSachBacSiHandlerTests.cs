using ClinicBooking.Application.Features.BacSi.Queries.DanhSachBacSi;
using ClinicBooking.Application.UnitTests.Common;
using BacSiEntity = ClinicBooking.Domain.Entities.BacSi;
using ClinicBooking.Domain.Enums;
using FluentAssertions;

namespace ClinicBooking.Application.UnitTests.Features.BacSi.Queries.DanhSachBacSi;

public sealed class DanhSachBacSiHandlerTests
{
    [Fact]
    public async Task Handle_FilterTheoChuyenKhoa_TraVeDungBacSi()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var ck1 = TestDataSeeder.SeedChuyenKhoa(db, "CK-UT-BacSi-LS-1");
        var ck2 = TestDataSeeder.SeedChuyenKhoa(db, "CK-UT-BacSi-LS-2");
        var tk1 = TestDataSeeder.SeedTaiKhoan(db, VaiTro.BacSi);
        var tk2 = TestDataSeeder.SeedTaiKhoan(db, VaiTro.BacSi);
        db.BacSi.AddRange(
            new BacSiEntity { IdTaiKhoan = tk1.IdTaiKhoan, IdChuyenKhoa = ck1.IdChuyenKhoa, HoTen = "Bac Si A", LoaiHopDong = LoaiHopDong.HopDong, TrangThai = TrangThaiBacSi.DangLam, NgayTao = DateTime.UtcNow },
            new BacSiEntity { IdTaiKhoan = tk2.IdTaiKhoan, IdChuyenKhoa = ck2.IdChuyenKhoa, HoTen = "Bac Si B", LoaiHopDong = LoaiHopDong.NoiTru, TrangThai = TrangThaiBacSi.DangLam, NgayTao = DateTime.UtcNow });
        await db.SaveChangesAsync();

        var handler = new DanhSachBacSiHandler(db);
        var result = await handler.Handle(new DanhSachBacSiQuery(IdChuyenKhoa: ck2.IdChuyenKhoa), CancellationToken.None);

        result.Should().ContainSingle(x => x.HoTen == "Bac Si B");
    }
}
