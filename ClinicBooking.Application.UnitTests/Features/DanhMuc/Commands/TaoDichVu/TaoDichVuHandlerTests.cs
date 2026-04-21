using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.DanhMuc.Commands.TaoDichVu;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.UnitTests.Features.DanhMuc.Commands.TaoDichVu;

public sealed class TaoDichVuHandlerTests
{
    [Fact]
    public async Task Handle_HopLe_TaoThanhCong()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var chuyenKhoa = new ChuyenKhoa
        {
            TenChuyenKhoa = "Noi Tong Quat Test",
            ThoiGianSlotMacDinh = 20,
            HienThi = true
        };
        db.ChuyenKhoa.Add(chuyenKhoa);
        await db.SaveChangesAsync();

        var handler = new TaoDichVuHandler(db);

        var id = await handler.Handle(
            new TaoDichVuCommand(chuyenKhoa.IdChuyenKhoa, "Kham tong quat", "Mo ta", 20, true),
            CancellationToken.None);

        id.Should().BeGreaterThan(0);
        var entity = await db.DichVu.AsNoTracking().FirstAsync(x => x.IdDichVu == id);
        entity.TenDichVu.Should().Be("Kham tong quat");
    }

    [Fact]
    public async Task Handle_IdChuyenKhoaKhongTonTai_ThrowNotFoundException()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var handler = new TaoDichVuHandler(db);

        var act = async () => await handler.Handle(
            new TaoDichVuCommand(999999, "DV", null, null, true),
            CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Khong tim thay chuyen khoa.");
    }
}
