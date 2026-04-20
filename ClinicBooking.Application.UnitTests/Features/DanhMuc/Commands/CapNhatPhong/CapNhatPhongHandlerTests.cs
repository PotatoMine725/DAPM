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

        var entity = new Phong { MaPhong = "P301", TenPhong = "Phong cu", TrangThai = true };
        db.Phong.Add(entity);
        await db.SaveChangesAsync();

        var handler = new CapNhatPhongHandler(db);
        var result = await handler.Handle(
            new CapNhatPhongCommand(entity.IdPhong, "P301A", "Phong moi", 25, "May ECG", false),
            CancellationToken.None);

        result.Should().Be(Unit.Value);
        var updated = await db.Phong.AsNoTracking().FirstAsync(x => x.IdPhong == entity.IdPhong);
        updated.MaPhong.Should().Be("P301A");
        updated.TenPhong.Should().Be("Phong moi");
        updated.TrangThai.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_TrungMaPhong_ThrowConflictException()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        db.Phong.AddRange(
            new Phong { MaPhong = "P401", TenPhong = "Phong 401", TrangThai = true },
            new Phong { MaPhong = "P402", TenPhong = "Phong 402", TrangThai = true });
        await db.SaveChangesAsync();

        var target = await db.Phong.FirstAsync(x => x.MaPhong == "P401");
        var handler = new CapNhatPhongHandler(db);

        var act = async () => await handler.Handle(
            new CapNhatPhongCommand(target.IdPhong, "P402", "Phong doi", null, null, true),
            CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("Ma phong da ton tai.");
    }
}
