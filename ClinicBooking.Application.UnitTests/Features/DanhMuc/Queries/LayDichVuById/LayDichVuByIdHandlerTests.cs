using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.DanhMuc.Queries.LayDichVuById;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Entities;
using FluentAssertions;

namespace ClinicBooking.Application.UnitTests.Features.DanhMuc.Queries.LayDichVuById;

public sealed class LayDichVuByIdHandlerTests
{
    [Fact]
    public async Task Handle_TonTai_TraVeDuLieu()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var chuyenKhoa = new ChuyenKhoa { TenChuyenKhoa = "Noi", ThoiGianSlotMacDinh = 20, HienThi = true };
        db.ChuyenKhoa.Add(chuyenKhoa);
        await db.SaveChangesAsync();

        var entity = new DichVu
        {
            IdChuyenKhoa = chuyenKhoa.IdChuyenKhoa,
            TenDichVu = "Kham Noi Tong Quat",
            MoTa = "Mo ta",
            ThoiGianUocTinh = 30,
            HienThi = true,
            NgayTao = DateTime.UtcNow
        };
        db.DichVu.Add(entity);
        await db.SaveChangesAsync();

        var handler = new LayDichVuByIdHandler(db);
        var result = await handler.Handle(new LayDichVuByIdQuery(entity.IdDichVu), CancellationToken.None);

        result.IdDichVu.Should().Be(entity.IdDichVu);
        result.TenDichVu.Should().Be("Kham Noi Tong Quat");
        result.TenChuyenKhoa.Should().Be("Noi");
    }

    [Fact]
    public async Task Handle_KhongTonTai_ThrowNotFoundException()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var handler = new LayDichVuByIdHandler(db);
        var act = async () => await handler.Handle(new LayDichVuByIdQuery(999999), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Khong tim thay dich vu.");
    }
}
