using ClinicBooking.Application.Features.BacSi.Commands.XoaBacSi;
using ClinicBooking.Application.UnitTests.Common;
using BacSiEntity = ClinicBooking.Domain.Entities.BacSi;
using ClinicBooking.Domain.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.UnitTests.Features.BacSi.Commands.XoaBacSi;

public sealed class XoaBacSiHandlerTests
{
    [Fact]
    public async Task Handle_HopLe_XoaThanhCong()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var tk = TestDataSeeder.SeedTaiKhoan(db, VaiTro.BacSi);
        var ck = TestDataSeeder.SeedChuyenKhoa(db, "CK-UT-BacSi-Xoa-20260422");
        var bacSi = new BacSiEntity
        {
            IdTaiKhoan = tk.IdTaiKhoan,
            IdChuyenKhoa = ck.IdChuyenKhoa,
            HoTen = "Bac Si UT Xoa",
            LoaiHopDong = LoaiHopDong.HopDong,
            TrangThai = TrangThaiBacSi.DangLam,
            NgayTao = DateTime.UtcNow
        };
        db.BacSi.Add(bacSi);
        await db.SaveChangesAsync();

        var handler = new XoaBacSiHandler(db);
        await handler.Handle(new XoaBacSiCommand(bacSi.IdBacSi), CancellationToken.None);

        var conTonTai = await db.BacSi.AnyAsync(x => x.IdBacSi == bacSi.IdBacSi);
        conTonTai.Should().BeFalse();
    }
}
