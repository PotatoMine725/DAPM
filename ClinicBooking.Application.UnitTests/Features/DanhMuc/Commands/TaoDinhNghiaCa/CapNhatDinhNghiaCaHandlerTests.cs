using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.DanhMuc.Commands.CapNhatDinhNghiaCa;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Entities;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.UnitTests.Features.DanhMuc.Commands.TaoDinhNghiaCa;

public sealed class CapNhatDinhNghiaCaHandlerTests
{
    [Fact]
    public async Task Handle_HopLe_CapNhatThanhCong()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var entity = new DinhNghiaCa
        {
            TenCa = "Ca cu",
            GioBatDauMacDinh = new TimeOnly(7, 0),
            GioKetThucMacDinh = new TimeOnly(11, 0),
            TrangThai = true
        };
        db.DinhNghiaCa.Add(entity);
        await db.SaveChangesAsync();

        var handler = new CapNhatDinhNghiaCaHandler(db);

        var result = await handler.Handle(
            new CapNhatDinhNghiaCaCommand(
                entity.IdDinhNghiaCa,
                "Ca moi",
                new TimeOnly(13, 0),
                new TimeOnly(17, 0),
                "Mo ta moi",
                false),
            CancellationToken.None);

        result.Should().Be(Unit.Value);

        var updated = await db.DinhNghiaCa.AsNoTracking()
            .FirstAsync(x => x.IdDinhNghiaCa == entity.IdDinhNghiaCa);
        updated.TenCa.Should().Be("Ca moi");
        updated.GioBatDauMacDinh.Should().Be(new TimeOnly(13, 0));
        updated.TrangThai.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_TrungTenCa_ThrowConflictException()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        db.DinhNghiaCa.AddRange(
            new DinhNghiaCa
            {
                TenCa = "Ca A",
                GioBatDauMacDinh = new TimeOnly(7, 0),
                GioKetThucMacDinh = new TimeOnly(11, 0),
                TrangThai = true
            },
            new DinhNghiaCa
            {
                TenCa = "Ca B",
                GioBatDauMacDinh = new TimeOnly(13, 0),
                GioKetThucMacDinh = new TimeOnly(17, 0),
                TrangThai = true
            });
        await db.SaveChangesAsync();

        var target = await db.DinhNghiaCa.FirstAsync(x => x.TenCa == "Ca A");
        var handler = new CapNhatDinhNghiaCaHandler(db);

        var act = async () => await handler.Handle(
            new CapNhatDinhNghiaCaCommand(
                target.IdDinhNghiaCa,
                "Ca B",
                new TimeOnly(7, 0),
                new TimeOnly(11, 0),
                null,
                true),
            CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("Ten ca da ton tai.");
    }
}
