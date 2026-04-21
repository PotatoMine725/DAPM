using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.DanhMuc.Commands.CapNhatPhong;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Entities;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.UnitTests.Features.DanhMuc.Commands.CapNhatPhong;

public sealed class CapNhatPhongHandlerTests
{
    [Fact]
    public async Task Handle_HopLe_CapNhatThanhCong()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var entity = new Phong { MaPhong = TestSeedSafeValues.MaPhongCapNhatGoc, TenPhong = "Phong cu", TrangThai = true };
        db.Phong.Add(entity);
        await db.SaveChangesAsync();

        var handler = new CapNhatPhongHandler(db);
        var result = await handler.Handle(
            new CapNhatPhongCommand(entity.IdPhong, TestSeedSafeValues.MaPhongCapNhatMoi, "Phong moi", 25, "May ECG", false),
            CancellationToken.None);

        result.Should().Be(Unit.Value);
        var updated = await db.Phong.AsNoTracking().FirstAsync(x => x.IdPhong == entity.IdPhong);
        updated.MaPhong.Should().Be(TestSeedSafeValues.MaPhongCapNhatMoi);
        updated.TenPhong.Should().Be("Phong moi");
        updated.TrangThai.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_TrungMaPhong_ThrowConflictException()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        db.Phong.AddRange(
            new Phong { MaPhong = TestSeedSafeValues.MaPhongTrungA, TenPhong = "Phong 401", TrangThai = true },
            new Phong { MaPhong = TestSeedSafeValues.MaPhongTrungB, TenPhong = "Phong 402", TrangThai = true });
        await db.SaveChangesAsync();

        var target = await db.Phong.FirstAsync(x => x.MaPhong == TestSeedSafeValues.MaPhongTrungA);
        var handler = new CapNhatPhongHandler(db);

        var act = async () => await handler.Handle(
            new CapNhatPhongCommand(target.IdPhong, TestSeedSafeValues.MaPhongTrungB, "Phong doi", null, null, true),
            CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("Ma phong da ton tai.");
    }
}
