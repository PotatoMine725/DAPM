using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.DanhMuc.Queries.LayChuyenKhoaById;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Entities;
using FluentAssertions;

namespace ClinicBooking.Application.UnitTests.Features.DanhMuc.Queries.LayChuyenKhoaById;

public sealed class LayChuyenKhoaByIdHandlerTests
{
    [Fact]
    public async Task Handle_TonTai_TraVeDuLieu()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var entity = new ChuyenKhoa
        {
            TenChuyenKhoa = "Noi Tim Mach",
            MoTa = "Mo ta test",
            ThoiGianSlotMacDinh = 20,
            HienThi = true
        };
        db.ChuyenKhoa.Add(entity);
        await db.SaveChangesAsync();

        var handler = new LayChuyenKhoaByIdHandler(db);
        var result = await handler.Handle(new LayChuyenKhoaByIdQuery(entity.IdChuyenKhoa), CancellationToken.None);

        result.IdChuyenKhoa.Should().Be(entity.IdChuyenKhoa);
        result.TenChuyenKhoa.Should().Be("Noi Tim Mach");
    }

    [Fact]
    public async Task Handle_KhongTonTai_ThrowNotFoundException()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var handler = new LayChuyenKhoaByIdHandler(db);
        var act = async () => await handler.Handle(new LayChuyenKhoaByIdQuery(999999), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Khong tim thay chuyen khoa.");
    }
}
