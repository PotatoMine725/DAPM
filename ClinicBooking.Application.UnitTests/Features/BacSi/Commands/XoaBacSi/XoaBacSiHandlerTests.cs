using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.BacSi.Commands.XoaBacSi;
using ClinicBooking.Application.UnitTests.Common;
using BacSiEntity = ClinicBooking.Domain.Entities.BacSi;
using ClinicBooking.Domain.Entities;
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

        var entity = await db.BacSi.AsNoTracking().FirstAsync(x => x.IdBacSi == bacSi.IdBacSi);
        entity.TrangThai.Should().Be(TrangThaiBacSi.NghiViec);
    }

    [Fact]
    public async Task Handle_BacSiKhongTonTai_ThrowNotFoundException()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var handler = new XoaBacSiHandler(db);

        var act = () => handler.Handle(new XoaBacSiCommand(999999), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>().WithMessage("Khong tim thay bac si.");
    }

    [Fact]
    public async Task Handle_ConCaLamViecTuongLai_ThrowConflictException()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var tk = TestDataSeeder.SeedTaiKhoan(db, VaiTro.BacSi);
        var ck = TestDataSeeder.SeedChuyenKhoa(db, "CK-UT-BacSi-Xoa-Conflict");
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

        db.CaLamViec.Add(new CaLamViec
        {
            IdBacSi = bacSi.IdBacSi,
            IdPhong = 1,
            IdChuyenKhoa = ck.IdChuyenKhoa,
            IdDinhNghiaCa = 1,
            NgayLamViec = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(1)),
            GioBatDau = new TimeOnly(8, 0),
            GioKetThuc = new TimeOnly(9, 0),
            ThoiGianSlot = 15,
            SoSlotToiDa = 4,
            SoSlotDaDat = 0,
            TrangThaiDuyet = TrangThaiDuyetCa.DaDuyet,
            NguonTaoCa = NguonTaoCa.TuDong,
            NgayTao = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        var handler = new XoaBacSiHandler(db);
        var act = () => handler.Handle(new XoaBacSiCommand(bacSi.IdBacSi), CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>().WithMessage("Bac si con ca lam viec sap toi, khong the xoa.");
    }
}
