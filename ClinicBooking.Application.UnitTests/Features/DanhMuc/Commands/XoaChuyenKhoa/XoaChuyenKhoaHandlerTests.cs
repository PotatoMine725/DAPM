using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.DanhMuc.Commands.XoaChuyenKhoa;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Entities;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.UnitTests.Features.DanhMuc.Commands.XoaChuyenKhoa;

public sealed class XoaChuyenKhoaHandlerTests
{
    [Fact]
    public async Task Handle_KhongDuocSuDung_XoaThanhCong()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var chuyenKhoa = new ChuyenKhoa
        {
            TenChuyenKhoa = "Chuyen khoa xoa",
            ThoiGianSlotMacDinh = 20,
            HienThi = true
        };
        db.ChuyenKhoa.Add(chuyenKhoa);
        await db.SaveChangesAsync();

        var handler = new XoaChuyenKhoaHandler(db);
        var result = await handler.Handle(new XoaChuyenKhoaCommand(chuyenKhoa.IdChuyenKhoa), CancellationToken.None);

        result.Should().Be(Unit.Value);
        var conTonTai = await db.ChuyenKhoa.AnyAsync(x => x.IdChuyenKhoa == chuyenKhoa.IdChuyenKhoa);
        conTonTai.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_KhongTonTai_ThrowNotFoundException()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var handler = new XoaChuyenKhoaHandler(db);

        var act = async () => await handler.Handle(new XoaChuyenKhoaCommand(999999), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Khong tim thay chuyen khoa.");
    }

    [Fact]
    public async Task Handle_DangDuocSuDung_ThrowConflictException()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var chuyenKhoa = new ChuyenKhoa
        {
            TenChuyenKhoa = "Chuyen khoa dang su dung",
            ThoiGianSlotMacDinh = 20,
            HienThi = true
        };
        db.ChuyenKhoa.Add(chuyenKhoa);
        await db.SaveChangesAsync();

        db.DichVu.Add(new DichVu
        {
            IdChuyenKhoa = chuyenKhoa.IdChuyenKhoa,
            TenDichVu = "Dich vu lien ket",
            NgayTao = DateTime.UtcNow,
            HienThi = true
        });
        await db.SaveChangesAsync();

        var handler = new XoaChuyenKhoaHandler(db);
        var act = async () => await handler.Handle(new XoaChuyenKhoaCommand(chuyenKhoa.IdChuyenKhoa), CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("Khong the xoa chuyen khoa dang duoc su dung.");
    }
}
