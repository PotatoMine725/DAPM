using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.DanhMuc.Queries.LayDinhNghiaCaById;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Entities;
using FluentAssertions;

namespace ClinicBooking.Application.UnitTests.Features.DanhMuc.Queries.LayDinhNghiaCaById;

public sealed class LayDinhNghiaCaByIdHandlerTests
{
    [Fact]
    public async Task Handle_TonTai_TraVeDuLieu()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var entity = new DinhNghiaCa
        {
            TenCa = "Ca sang",
            GioBatDauMacDinh = new TimeOnly(7, 0),
            GioKetThucMacDinh = new TimeOnly(11, 0),
            MoTa = "Mo ta test",
            TrangThai = true
        };
        db.DinhNghiaCa.Add(entity);
        await db.SaveChangesAsync();

        var handler = new LayDinhNghiaCaByIdHandler(db);
        var result = await handler.Handle(new LayDinhNghiaCaByIdQuery(entity.IdDinhNghiaCa), CancellationToken.None);

        result.IdDinhNghiaCa.Should().Be(entity.IdDinhNghiaCa);
        result.TenCa.Should().Be("Ca sang");
    }

    [Fact]
    public async Task Handle_KhongTonTai_ThrowNotFoundException()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var handler = new LayDinhNghiaCaByIdHandler(db);
        var act = async () => await handler.Handle(new LayDinhNghiaCaByIdQuery(999999), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Khong tim thay dinh nghia ca.");
    }
}
