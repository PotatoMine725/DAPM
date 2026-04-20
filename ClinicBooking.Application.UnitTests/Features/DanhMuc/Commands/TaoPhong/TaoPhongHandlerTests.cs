using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.DanhMuc.Commands.TaoPhong;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.UnitTests.Features.DanhMuc.Commands.TaoPhong;

public sealed class TaoPhongHandlerTests
{
    [Fact]
    public async Task Handle_TaoMoi_TraVeId()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var handler = new TaoPhongHandler(db);

        var id = await handler.Handle(
            new TaoPhongCommand("P100", "Phong Kham 100", 20, "May sieu am", true),
            CancellationToken.None);

        id.Should().BeGreaterThan(0);
        var entity = await db.Phong.AsNoTracking().FirstAsync(x => x.IdPhong == id);
        entity.MaPhong.Should().Be("P100");
        entity.TenPhong.Should().Be("Phong Kham 100");
    }

    [Fact]
    public async Task Handle_TrungMaPhong_ThrowConflictException()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        db.Phong.Add(new Phong
        {
            MaPhong = "P200",
            TenPhong = "Phong Co San",
            TrangThai = true
        });
        await db.SaveChangesAsync();

        var handler = new TaoPhongHandler(db);

        var act = async () => await handler.Handle(
            new TaoPhongCommand("P200", "Phong Moi", null, null, true),
            CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("Ma phong da ton tai.");
    }
}
