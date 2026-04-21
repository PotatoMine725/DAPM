using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.DanhMuc.Queries.LayPhongById;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Entities;
using FluentAssertions;

namespace ClinicBooking.Application.UnitTests.Features.DanhMuc.Queries.LayPhongById;

public sealed class LayPhongByIdHandlerTests
{
    [Fact]
    public async Task Handle_TonTai_TraVeDuLieu()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var entity = new Phong
        {
            MaPhong = "UT-P101",
            TenPhong = "Phong Kham Tong Quat",
            SucChua = 10,
            TrangBi = "May do huyet ap",
            TrangThai = true
        };
        db.Phong.Add(entity);
        await db.SaveChangesAsync();

        var handler = new LayPhongByIdHandler(db);
        var result = await handler.Handle(new LayPhongByIdQuery(entity.IdPhong), CancellationToken.None);

        result.IdPhong.Should().Be(entity.IdPhong);
        result.MaPhong.Should().Be("UT-P101");
    }

    [Fact]
    public async Task Handle_KhongTonTai_ThrowNotFoundException()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var handler = new LayPhongByIdHandler(db);
        var act = async () => await handler.Handle(new LayPhongByIdQuery(999999), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Khong tim thay phong.");
    }
}
