using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.Thuoc.Commands.XoaThuoc;
using ClinicBooking.Application.UnitTests.Common;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.UnitTests.Features.Thuoc.Commands.XoaThuoc;

public sealed class XoaThuocHandlerTests
{
    [Fact]
    public async Task Handle_HopLe_XoaThanhCong()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var thuoc = new ClinicBooking.Domain.Entities.Thuoc { TenThuoc = "THUOC-UT-Xoa-20260422" };
        db.Thuoc.Add(thuoc);
        await db.SaveChangesAsync();
        var idThuoc = thuoc.IdThuoc;

        var handler = new XoaThuocHandler(db);
        await handler.Handle(new XoaThuocCommand(idThuoc), CancellationToken.None);

        var conTonTai = await db.Thuoc.AnyAsync(x => x.IdThuoc == idThuoc);
        conTonTai.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_IdKhongTonTai_ThrowNotFoundException()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var handler = new XoaThuocHandler(db);

        var act = async () => await handler.Handle(new XoaThuocCommand(9999), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Khong tim thay thuoc.");
    }
}
