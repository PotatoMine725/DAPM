using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.DanhMuc.Commands.XoaDichVu;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Entities;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.UnitTests.Features.DanhMuc.Commands.XoaDichVu;

public sealed class XoaDichVuHandlerTests
{
    [Fact]
    public async Task Handle_KhongDuocSuDung_XoaThanhCong()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var chuyenKhoa = new ChuyenKhoa { TenChuyenKhoa = "CK xoa", ThoiGianSlotMacDinh = 20, HienThi = true };
        db.ChuyenKhoa.Add(chuyenKhoa);
        await db.SaveChangesAsync();

        var dichVu = new DichVu
        {
            IdChuyenKhoa = chuyenKhoa.IdChuyenKhoa,
            TenDichVu = "DV can xoa",
            NgayTao = DateTime.UtcNow,
            HienThi = true
        };
        db.DichVu.Add(dichVu);
        await db.SaveChangesAsync();

        var handler = new XoaDichVuHandler(db);
        var result = await handler.Handle(new XoaDichVuCommand(dichVu.IdDichVu), CancellationToken.None);

        result.Should().Be(Unit.Value);
        var conTonTai = await db.DichVu.AnyAsync(x => x.IdDichVu == dichVu.IdDichVu);
        conTonTai.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_KhongTonTai_ThrowNotFoundException()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var handler = new XoaDichVuHandler(db);

        var act = async () => await handler.Handle(new XoaDichVuCommand(999999), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Khong tim thay dich vu.");
    }
}
