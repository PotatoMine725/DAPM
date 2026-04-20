using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.Thuoc.Commands.TaoThuoc;
using ClinicBooking.Application.UnitTests.Common;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.UnitTests.Features.Thuoc.Commands.TaoThuoc;

public sealed class TaoThuocHandlerTests
{
    [Fact]
    public async Task Handle_HopLe_TaoThanhCong()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var handler = new TaoThuocHandler(db);

        var id = await handler.Handle(
            new TaoThuocCommand("Paracetamol 500mg", "Paracetamol", "Vien", "Sau an"),
            CancellationToken.None);

        id.Should().BeGreaterThan(0);
        var entity = await db.Thuoc.AsNoTracking().FirstAsync(x => x.IdThuoc == id);
        entity.TenThuoc.Should().Be("Paracetamol 500mg");
    }

    [Fact]
    public async Task Handle_TrungTen_ThrowConflictException()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        db.Thuoc.Add(new ClinicBooking.Domain.Entities.Thuoc { TenThuoc = "Paracetamol 500mg" });
        await db.SaveChangesAsync();

        var handler = new TaoThuocHandler(db);
        var act = async () => await handler.Handle(
            new TaoThuocCommand("Paracetamol 500mg", null, null, null),
            CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("Ten thuoc da ton tai.");
    }
}
