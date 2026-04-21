using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.DanhMuc.Commands.CapNhatDichVu;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Entities;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.UnitTests.Features.DanhMuc.Commands.CapNhatDichVu;

public sealed class CapNhatDichVuHandlerTests
{
    [Fact]
    public async Task Handle_HopLe_CapNhatThanhCong()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var chuyenKhoa = new ChuyenKhoa { TenChuyenKhoa = "CK1", ThoiGianSlotMacDinh = 20, HienThi = true };
        db.ChuyenKhoa.Add(chuyenKhoa);
        await db.SaveChangesAsync();

        var dichVu = new DichVu
        {
            IdChuyenKhoa = chuyenKhoa.IdChuyenKhoa,
            TenDichVu = "DV cu",
            NgayTao = DateTime.UtcNow,
            HienThi = true
        };
        db.DichVu.Add(dichVu);
        await db.SaveChangesAsync();

        var handler = new CapNhatDichVuHandler(db);
        var result = await handler.Handle(
            new CapNhatDichVuCommand(dichVu.IdDichVu, chuyenKhoa.IdChuyenKhoa, "DV moi", "Mo ta moi", 25, false),
            CancellationToken.None);

        result.Should().Be(Unit.Value);
        var updated = await db.DichVu.AsNoTracking().FirstAsync(x => x.IdDichVu == dichVu.IdDichVu);
        updated.TenDichVu.Should().Be("DV moi");
        updated.HienThi.Should().BeFalse();
    }
}
