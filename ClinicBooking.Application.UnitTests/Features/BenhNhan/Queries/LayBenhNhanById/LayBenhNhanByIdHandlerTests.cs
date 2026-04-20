using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.BenhNhan.Queries.LayBenhNhanById;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.UnitTests.Features.BenhNhan.Queries.LayBenhNhanById;

public sealed class LayBenhNhanByIdHandlerTests
{
    [Fact]
    public async Task Handle_TonTai_TraVeDuLieu()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var taiKhoan = new ClinicBooking.Domain.Entities.TaiKhoan
        {
            TenDangNhap = "a",
            Email = "a@example.com",
            SoDienThoai = "0912345678",
            MatKhau = "hash",
            VaiTro = VaiTro.BenhNhan,
            TrangThai = true,
            NgayTao = DateTime.UtcNow
        };
        db.TaiKhoan.Add(taiKhoan);
        await db.SaveChangesAsync();

        db.BenhNhan.Add(new ClinicBooking.Domain.Entities.BenhNhan
        {
            IdTaiKhoan = taiKhoan.IdTaiKhoan,
            HoTen = "Nguyen Van A",
            NgayTao = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        var idBenhNhan = await db.BenhNhan.Select(x => x.IdBenhNhan).FirstAsync();
        var handler = new LayBenhNhanByIdHandler(db);
        var result = await handler.Handle(new LayBenhNhanByIdQuery(idBenhNhan), CancellationToken.None);

        result.IdBenhNhan.Should().Be(idBenhNhan);
        result.HoTen.Should().Be("Nguyen Van A");
    }

    [Fact]
    public async Task Handle_KhongTonTai_ThrowNotFoundException()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var handler = new LayBenhNhanByIdHandler(db);

        var act = async () => await handler.Handle(new LayBenhNhanByIdQuery(9999), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Khong tim thay benh nhan.");
    }
}
