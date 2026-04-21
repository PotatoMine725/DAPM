using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.DanhMuc.Commands.TaoDinhNghiaCa;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.UnitTests.Features.DanhMuc.Commands.TaoDinhNghiaCa;

public sealed class TaoDinhNghiaCaHandlerTests
{
    [Fact]
    public async Task Handle_TaoMoi_TraVeId()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var handler = new TaoDinhNghiaCaHandler(db);

        var id = await handler.Handle(
            new TaoDinhNghiaCaCommand(
                "Ca Test",
                new TimeOnly(8, 0),
                new TimeOnly(12, 0),
                "Mo ta test",
                true),
            CancellationToken.None);

        id.Should().BeGreaterThan(0);

        var entity = await db.DinhNghiaCa.AsNoTracking()
            .FirstAsync(x => x.IdDinhNghiaCa == id);

        entity.TenCa.Should().Be("Ca Test");
        entity.GioBatDauMacDinh.Should().Be(new TimeOnly(8, 0));
        entity.GioKetThucMacDinh.Should().Be(new TimeOnly(12, 0));
    }

    [Fact]
    public async Task Handle_TrungTenCa_ThrowConflictException()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        db.DinhNghiaCa.Add(new DinhNghiaCa
        {
            TenCa = "Ca Trung",
            GioBatDauMacDinh = new TimeOnly(7, 0),
            GioKetThucMacDinh = new TimeOnly(11, 0),
            TrangThai = true
        });
        await db.SaveChangesAsync();

        var handler = new TaoDinhNghiaCaHandler(db);

        var act = async () => await handler.Handle(
            new TaoDinhNghiaCaCommand(
                "Ca Trung",
                new TimeOnly(13, 0),
                new TimeOnly(17, 0),
                null,
                true),
            CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("Ten ca da ton tai.");
    }
}
