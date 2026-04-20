using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.Thuoc.Commands.CapNhatThuoc;
using ClinicBooking.Application.UnitTests.Common;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.UnitTests.Features.Thuoc.Commands.CapNhatThuoc;

public sealed class CapNhatThuocHandlerTests
{
    [Fact]
    public async Task Handle_HopLe_CapNhatThanhCong()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        db.Thuoc.Add(new ClinicBooking.Domain.Entities.Thuoc { TenThuoc = "Paracetamol 500mg", DonVi = "Vien" });
        await db.SaveChangesAsync();

        var idThuoc = await db.Thuoc.Select(x => x.IdThuoc).FirstAsync();
        var handler = new CapNhatThuocHandler(db);

        await handler.Handle(
            new CapNhatThuocCommand(idThuoc, "Paracetamol 650mg", "Paracetamol", "Vien", "Moi"),
            CancellationToken.None);

        var entity = await db.Thuoc.AsNoTracking().FirstAsync(x => x.IdThuoc == idThuoc);
        entity.TenThuoc.Should().Be("Paracetamol 650mg");
        entity.GhiChu.Should().Be("Moi");
    }

    [Fact]
    public async Task Handle_IdKhongTonTai_ThrowNotFoundException()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var handler = new CapNhatThuocHandler(db);

        var act = async () => await handler.Handle(
            new CapNhatThuocCommand(9999, "A", null, null, null),
            CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Khong tim thay thuoc.");
    }
}
