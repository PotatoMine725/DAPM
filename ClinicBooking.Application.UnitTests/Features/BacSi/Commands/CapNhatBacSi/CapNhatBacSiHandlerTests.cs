using ClinicBooking.Application.Features.BacSi.Commands.CapNhatBacSi;
using ClinicBooking.Application.UnitTests.Common;
using BacSiEntity = ClinicBooking.Domain.Entities.BacSi;
using ClinicBooking.Domain.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.UnitTests.Features.BacSi.Commands.CapNhatBacSi;

public sealed class CapNhatBacSiHandlerTests
{
    [Fact]
    public async Task Handle_HopLe_CapNhatThanhCong()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var tk1 = TestDataSeeder.SeedTaiKhoan(db, VaiTro.BacSi);
        var ck1 = TestDataSeeder.SeedChuyenKhoa(db, "CK-UT-BacSi-CapNhat-1");
        var ck2 = TestDataSeeder.SeedChuyenKhoa(db, "CK-UT-BacSi-CapNhat-2");
        var bacSi = new BacSiEntity
        {
            IdTaiKhoan = tk1.IdTaiKhoan,
            IdChuyenKhoa = ck1.IdChuyenKhoa,
            HoTen = "Bac Si UT",
            LoaiHopDong = LoaiHopDong.HopDong,
            TrangThai = TrangThaiBacSi.DangLam,
            NgayTao = DateTime.UtcNow
        };
        db.BacSi.Add(bacSi);
        await db.SaveChangesAsync();

        var handler = new CapNhatBacSiHandler(db);
        await handler.Handle(new CapNhatBacSiCommand(
            bacSi.IdBacSi,
            ck2.IdChuyenKhoa,
            "Bac Si UT Updated",
            null,
            "Bac si noi tru",
            8,
            "Mo ta moi",
            nameof(LoaiHopDong.NoiTru),
            nameof(TrangThaiBacSi.TamNghi)), CancellationToken.None);

        var entity = await db.BacSi.AsNoTracking().Include(x => x.ChuyenKhoa).FirstAsync(x => x.IdBacSi == bacSi.IdBacSi);
        entity.HoTen.Should().Be("Bac Si UT Updated");
        entity.ChuyenKhoa.TenChuyenKhoa.Should().Be("CK-UT-BacSi-CapNhat-2");
        entity.TrangThai.Should().Be(TrangThaiBacSi.TamNghi);
    }
}
