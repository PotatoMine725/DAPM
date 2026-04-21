using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.DanhMuc.Commands.CapNhatChuyenKhoa;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Entities;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.UnitTests.Features.DanhMuc.Commands.CapNhatChuyenKhoa;

public sealed class CapNhatChuyenKhoaHandlerTests
{
    [Fact]
    public async Task Handle_HopLe_CapNhatThanhCong()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var chuyenKhoa = new ChuyenKhoa
        {
            TenChuyenKhoa = "UT Noi Tong Quat",
            MoTa = "Mo ta cu",
            ThoiGianSlotMacDinh = 20,
            GioMoDatLich = new TimeOnly(8, 0),
            GioDongDatLich = new TimeOnly(17, 0),
            HienThi = true
        };

        db.ChuyenKhoa.Add(chuyenKhoa);
        await db.SaveChangesAsync();

        var handler = new CapNhatChuyenKhoaHandler(db);
        var result = await handler.Handle(
            new CapNhatChuyenKhoaCommand(
                chuyenKhoa.IdChuyenKhoa,
                "UT Noi Tong Quat Cap Nhat",
                "Mo ta moi",
                30,
                new TimeOnly(7, 30),
                new TimeOnly(18, 0),
                false),
            CancellationToken.None);

        result.Should().Be(Unit.Value);
        var updated = await db.ChuyenKhoa.AsNoTracking().FirstAsync(x => x.IdChuyenKhoa == chuyenKhoa.IdChuyenKhoa);
        updated.TenChuyenKhoa.Should().Be("UT Noi Tong Quat Cap Nhat");
        updated.MoTa.Should().Be("Mo ta moi");
        updated.ThoiGianSlotMacDinh.Should().Be(30);
        updated.GioMoDatLich.Should().Be(new TimeOnly(7, 30));
        updated.GioDongDatLich.Should().Be(new TimeOnly(18, 0));
        updated.HienThi.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_KhongTonTai_ThrowNotFoundException()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var handler = new CapNhatChuyenKhoaHandler(db);

        var act = async () => await handler.Handle(
            new CapNhatChuyenKhoaCommand(
                999999,
                "Khong ton tai",
                null,
                20,
                null,
                null,
                true),
            CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Khong tim thay chuyen khoa.");
    }

    [Fact]
    public async Task Handle_TrungTen_ThrowConflictException()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        db.ChuyenKhoa.AddRange(
            new ChuyenKhoa
            {
                TenChuyenKhoa = "Da ton tai",
                ThoiGianSlotMacDinh = 20,
                HienThi = true
            },
            new ChuyenKhoa
            {
                TenChuyenKhoa = "Can cap nhat",
                ThoiGianSlotMacDinh = 20,
                HienThi = true
            });
        await db.SaveChangesAsync();

        var entityCanCapNhat = await db.ChuyenKhoa.AsNoTracking().FirstAsync(x => x.TenChuyenKhoa == "Can cap nhat");
        var handler = new CapNhatChuyenKhoaHandler(db);

        var act = async () => await handler.Handle(
            new CapNhatChuyenKhoaCommand(
                entityCanCapNhat.IdChuyenKhoa,
                "Da ton tai",
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
