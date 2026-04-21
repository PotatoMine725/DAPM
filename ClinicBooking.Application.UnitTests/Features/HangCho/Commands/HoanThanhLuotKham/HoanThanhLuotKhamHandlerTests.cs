using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.HangCho.Commands.HoanThanhLuotKham;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Entities;
using ClinicBooking.Domain.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.UnitTests.Features.HangCho.Commands.HoanThanhLuotKham;

public sealed class HoanThanhLuotKhamHandlerTests
{
    [Fact]
    public async Task Handle_LuotDangKham_DanhDauHoanThanh()
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

        var handler = new HoanThanhLuotKhamHandler(db);
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
        var handler = new HoanThanhLuotKhamHandler(db);

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

        var handler = new HoanThanhLuotKhamHandler(db);
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

        var handler = new HoanThanhLuotKhamHandler(db);
        var act = async () => await handler.Handle(new HoanThanhLuotKhamCommand(hc.IdHangCho), CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("Chi co the hoan thanh luot dang kham.");
    }
}
