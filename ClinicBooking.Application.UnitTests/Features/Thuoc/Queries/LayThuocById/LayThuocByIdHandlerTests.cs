using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.Thuoc.Queries.LayThuocById;
using ClinicBooking.Application.UnitTests.Common;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.UnitTests.Features.Thuoc.Queries.LayThuocById;

public sealed class LayThuocByIdHandlerTests
{
    [Fact]
    public async Task Handle_TonTai_TraVeDuLieu()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        db.Thuoc.Add(new ClinicBooking.Domain.Entities.Thuoc { TenThuoc = "Paracetamol 500mg", HoatChat = "Paracetamol" });
        await db.SaveChangesAsync();
        var idThuoc = await db.Thuoc.Select(x => x.IdThuoc).FirstAsync();

        var handler = new LayThuocByIdHandler(db);
        var result = await handler.Handle(new LayThuocByIdQuery(idThuoc), CancellationToken.None);

        result.IdThuoc.Should().Be(idThuoc);
        result.TenThuoc.Should().Be("Paracetamol 500mg");
    }

    [Fact]
    public async Task Handle_KhongTonTai_ThrowNotFoundException()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var handler = new LayThuocByIdHandler(db);

        var act = async () => await handler.Handle(new LayThuocByIdQuery(9999), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Khong tim thay thuoc.");
    }
}
