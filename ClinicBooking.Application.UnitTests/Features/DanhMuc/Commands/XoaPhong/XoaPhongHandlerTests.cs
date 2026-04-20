using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.DanhMuc.Commands.XoaPhong;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Entities;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.UnitTests.Features.DanhMuc.Commands.XoaPhong;

public sealed class XoaPhongHandlerTests
{
    [Fact]
    public async Task Handle_KhongDuocSuDung_XoaThanhCong()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var phong = new Phong { MaPhong = "PX01", TenPhong = "Phong xoa", TrangThai = true };
        db.Phong.Add(phong);
        await db.SaveChangesAsync();

        var handler = new XoaPhongHandler(db);
        var result = await handler.Handle(new XoaPhongCommand(phong.IdPhong), CancellationToken.None);

        result.Should().Be(Unit.Value);
        var conTonTai = await db.Phong.AnyAsync(x => x.IdPhong == phong.IdPhong);
        conTonTai.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_KhongTonTai_ThrowNotFoundException()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var handler = new XoaPhongHandler(db);

        var act = async () => await handler.Handle(new XoaPhongCommand(999999), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Khong tim thay phong.");
    }
}
