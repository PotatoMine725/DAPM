using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.HangCho.Commands.HoanThanhLuotKham;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Entities;
using ClinicBooking.Domain.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace ClinicBooking.Application.UnitTests.Features.HangCho.Commands.HoanThanhLuotKham;

public sealed class HoanThanhLuotKhamHandlerTests
{
    private static ICurrentUserService CurrentUserOf(VaiTro vaiTro, int? idTaiKhoan)
    {
        var cu = Substitute.For<ICurrentUserService>();
        cu.VaiTro.Returns(vaiTro);
        cu.IdTaiKhoan.Returns(idTaiKhoan);
        cu.DaXacThuc.Returns(idTaiKhoan.HasValue);
        return cu;
    }

    private static int LayIdTaiKhoanBacSiCuaCa(ClinicBooking.Infrastructure.Persistence.AppDbContext db, int idCaLamViec)
    {
        return db.CaLamViec
            .AsNoTracking()
            .Where(x => x.IdCaLamViec == idCaLamViec)
            .Join(db.BacSi.AsNoTracking(), ca => ca.IdBacSi, bs => bs.IdBacSi, (_, bs) => bs.IdTaiKhoan)
            .First();
    }

    [Fact]
    public async Task Handle_LuotDangKham_BacSiPhuTrach_DanhDauHoanThanh()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var bn = TestDataSeeder.SeedBenhNhan(db);
        var ca = TestDataSeeder.SeedCaLamViec(db);
        var lh = TestDataSeeder.SeedLichHen(db, bn.IdBenhNhan, ca.IdCaLamViec, trangThai: TrangThaiLichHen.DangKham);
        var hc = new ClinicBooking.Domain.Entities.HangCho
        {
            IdCaLamViec = ca.IdCaLamViec,
            IdLichHen = lh.IdLichHen,
            SoThuTu = 1,
            TrangThai = TrangThaiHangCho.DangKham,
            NgayCheckIn = DateTime.UtcNow
        };
        db.HangCho.Add(hc);
        await db.SaveChangesAsync();

        var idTkBacSi = LayIdTaiKhoanBacSiCuaCa(db, ca.IdCaLamViec);
        var handler = new HoanThanhLuotKhamHandler(db, CurrentUserOf(VaiTro.BacSi, idTkBacSi));
        await handler.Handle(new HoanThanhLuotKhamCommand(hc.IdHangCho), CancellationToken.None);

        var hcSau = await db.HangCho.AsNoTracking().FirstAsync(x => x.IdHangCho == hc.IdHangCho);
        var lhSau = await db.LichHen.AsNoTracking().FirstAsync(x => x.IdLichHen == lh.IdLichHen);
        hcSau.TrangThai.Should().Be(TrangThaiHangCho.HoanThanh);
        lhSau.TrangThai.Should().Be(TrangThaiLichHen.HoanThanh);
    }

    [Fact]
    public async Task Handle_HangChoKhongTonTai_ThrowNotFoundException()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var handler = new HoanThanhLuotKhamHandler(db, CurrentUserOf(VaiTro.BacSi, 1));

        var act = async () => await handler.Handle(new HoanThanhLuotKhamCommand(999), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_LuotDaHoanThanh_ThrowConflictException()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var bn = TestDataSeeder.SeedBenhNhan(db);
        var ca = TestDataSeeder.SeedCaLamViec(db);
        var lh = TestDataSeeder.SeedLichHen(db, bn.IdBenhNhan, ca.IdCaLamViec, trangThai: TrangThaiLichHen.HoanThanh);
        var hc = new ClinicBooking.Domain.Entities.HangCho
        {
            IdCaLamViec = ca.IdCaLamViec,
            IdLichHen = lh.IdLichHen,
            SoThuTu = 1,
            TrangThai = TrangThaiHangCho.HoanThanh,
            NgayCheckIn = DateTime.UtcNow
        };
        db.HangCho.Add(hc);
        await db.SaveChangesAsync();

        var idTkBacSi = LayIdTaiKhoanBacSiCuaCa(db, ca.IdCaLamViec);
        var handler = new HoanThanhLuotKhamHandler(db, CurrentUserOf(VaiTro.BacSi, idTkBacSi));
        var act = async () => await handler.Handle(new HoanThanhLuotKhamCommand(hc.IdHangCho), CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("Luot kham da duoc danh dau hoan thanh truoc do.");
    }

    [Fact]
    public async Task Handle_LuotChoKham_ThrowConflictException()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var bn = TestDataSeeder.SeedBenhNhan(db);
        var ca = TestDataSeeder.SeedCaLamViec(db);
        var lh = TestDataSeeder.SeedLichHen(db, bn.IdBenhNhan, ca.IdCaLamViec, trangThai: TrangThaiLichHen.DaXacNhan);
        var hc = new ClinicBooking.Domain.Entities.HangCho
        {
            IdCaLamViec = ca.IdCaLamViec,
            IdLichHen = lh.IdLichHen,
            SoThuTu = 1,
            TrangThai = TrangThaiHangCho.ChoKham,
            NgayCheckIn = DateTime.UtcNow
        };
        db.HangCho.Add(hc);
        await db.SaveChangesAsync();

        var idTkBacSi = LayIdTaiKhoanBacSiCuaCa(db, ca.IdCaLamViec);
        var handler = new HoanThanhLuotKhamHandler(db, CurrentUserOf(VaiTro.BacSi, idTkBacSi));
        var act = async () => await handler.Handle(new HoanThanhLuotKhamCommand(hc.IdHangCho), CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("Chi co the hoan thanh luot dang kham.");
    }

    [Fact]
    public async Task Handle_VaiTroKhongPhaiBacSi_ThrowForbiddenException()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var bn = TestDataSeeder.SeedBenhNhan(db);
        var ca = TestDataSeeder.SeedCaLamViec(db);
        var lh = TestDataSeeder.SeedLichHen(db, bn.IdBenhNhan, ca.IdCaLamViec, trangThai: TrangThaiLichHen.DangKham);
        var hc = new ClinicBooking.Domain.Entities.HangCho
        {
            IdCaLamViec = ca.IdCaLamViec,
            IdLichHen = lh.IdLichHen,
            SoThuTu = 1,
            TrangThai = TrangThaiHangCho.DangKham,
            NgayCheckIn = DateTime.UtcNow
        };
        db.HangCho.Add(hc);
        await db.SaveChangesAsync();

        var tkLeTan = TestDataSeeder.SeedTaiKhoan(db, VaiTro.LeTan);
        var handler = new HoanThanhLuotKhamHandler(db, CurrentUserOf(VaiTro.LeTan, tkLeTan.IdTaiKhoan));
        var act = async () => await handler.Handle(new HoanThanhLuotKhamCommand(hc.IdHangCho), CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>()
            .WithMessage("Chi bac si phu trach ca moi duoc hoan thanh luot kham.");
    }

    [Fact]
    public async Task Handle_BacSiKhongPhuTrachCa_ThrowForbiddenException()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var bn = TestDataSeeder.SeedBenhNhan(db);
        var ca = TestDataSeeder.SeedCaLamViec(db);
        var lh = TestDataSeeder.SeedLichHen(db, bn.IdBenhNhan, ca.IdCaLamViec, trangThai: TrangThaiLichHen.DangKham);
        var hc = new ClinicBooking.Domain.Entities.HangCho
        {
            IdCaLamViec = ca.IdCaLamViec,
            IdLichHen = lh.IdLichHen,
            SoThuTu = 1,
            TrangThai = TrangThaiHangCho.DangKham,
            NgayCheckIn = DateTime.UtcNow
        };
        db.HangCho.Add(hc);
        await db.SaveChangesAsync();

        // BacSi khac — khong phai bac si phu trach ca nay.
        var bsKhac = TestDataSeeder.SeedBacSi(db);
        var handler = new HoanThanhLuotKhamHandler(db, CurrentUserOf(VaiTro.BacSi, bsKhac.IdTaiKhoan));
        var act = async () => await handler.Handle(new HoanThanhLuotKhamCommand(hc.IdHangCho), CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>()
            .WithMessage("Ban khong phu trach ca lam viec nay.");
    }
}
