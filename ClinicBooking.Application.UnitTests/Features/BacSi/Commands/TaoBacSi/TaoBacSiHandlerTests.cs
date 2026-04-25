using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.BacSi.Commands.TaoBacSi;
using ClinicBooking.Application.UnitTests.Common;
using BacSiEntity = ClinicBooking.Domain.Entities.BacSi;
using ClinicBooking.Domain.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.UnitTests.Features.BacSi.Commands.TaoBacSi;

public sealed class TaoBacSiHandlerTests
{
    [Fact]
    public async Task Handle_HopLe_TaoThanhCong()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var tk = TestDataSeeder.SeedTaiKhoan(db, VaiTro.BacSi);
        var ck = TestDataSeeder.SeedChuyenKhoa(db, "CK-UT-BacSi-Tao-20260422");
        var handler = new TaoBacSiHandler(db);

        var id = await handler.Handle(new TaoBacSiCommand(
            tk.IdTaiKhoan,
            ck.IdChuyenKhoa,
            "Bac Si UT Tao",
            null,
            "Dai hoc y",
            5,
            "Mo ta UT",
            nameof(LoaiHopDong.HopDong),
            nameof(TrangThaiBacSi.DangLam)), CancellationToken.None);

        id.Should().BeGreaterThan(0);
        var entity = await db.BacSi.AsNoTracking().Include(x => x.ChuyenKhoa).FirstAsync(x => x.IdBacSi == id);
        entity.HoTen.Should().Be("Bac Si UT Tao");
        entity.ChuyenKhoa.TenChuyenKhoa.Should().Be("CK-UT-BacSi-Tao-20260422");
    }

    [Fact]
    public async Task Handle_TaiKhoanKhongTonTai_ThrowNotFoundException()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var ck = TestDataSeeder.SeedChuyenKhoa(db, "CK-UT-BacSi-Tao-404");
        var handler = new TaoBacSiHandler(db);

        var act = () => handler.Handle(new TaoBacSiCommand(
            999999,
            ck.IdChuyenKhoa,
            "Bac Si UT Tao",
            null,
            "Dai hoc y",
            5,
            "Mo ta UT",
            nameof(LoaiHopDong.HopDong),
            nameof(TrangThaiBacSi.DangLam)), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>().WithMessage("Khong tim thay tai khoan.");
    }

    [Fact]
    public async Task Handle_ChuyenKhoaKhongTonTai_ThrowNotFoundException()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var tk = TestDataSeeder.SeedTaiKhoan(db, VaiTro.BacSi);
        var handler = new TaoBacSiHandler(db);

        var act = () => handler.Handle(new TaoBacSiCommand(
            tk.IdTaiKhoan,
            999999,
            "Bac Si UT Tao",
            null,
            "Dai hoc y",
            5,
            "Mo ta UT",
            nameof(LoaiHopDong.HopDong),
            nameof(TrangThaiBacSi.DangLam)), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>().WithMessage("Khong tim thay chuyen khoa.");
    }
}
