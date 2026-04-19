using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.DanhMuc.Commands.TaoChuyenKhoa;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.UnitTests.Features.DanhMuc.Commands.TaoChuyenKhoa;

public sealed class TaoChuyenKhoaHandlerTests
{
    [Fact]
    public async Task Handle_TaoMoi_TraVeId()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var handler = new TaoChuyenKhoaHandler(db);

        var id = await handler.Handle(
            new TaoChuyenKhoaCommand(
                "Chuyen Khoa Test",
                "Mo ta",
                15,
                new TimeOnly(8, 0),
                new TimeOnly(17, 0),
                true),
            CancellationToken.None);

        id.Should().BeGreaterThan(0);
        var entity = await db.ChuyenKhoa.AsNoTracking().FirstAsync(x => x.IdChuyenKhoa == id);
        entity.TenChuyenKhoa.Should().Be("Chuyen Khoa Test");
        entity.ThoiGianSlotMacDinh.Should().Be(15);
    }

    [Fact]
    public async Task Handle_TrungTen_ThrowConflictException()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        db.ChuyenKhoa.Add(new ChuyenKhoa
        {
            TenChuyenKhoa = "Trung Ten",
            ThoiGianSlotMacDinh = 20,
            HienThi = true
        });
        await db.SaveChangesAsync();

        var handler = new TaoChuyenKhoaHandler(db);

        var act = async () => await handler.Handle(
            new TaoChuyenKhoaCommand(
                "Trung Ten",
                null,
                20,
                null,
                null,
                true),
            CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("Ten chuyen khoa da ton tai.");
    }
}
