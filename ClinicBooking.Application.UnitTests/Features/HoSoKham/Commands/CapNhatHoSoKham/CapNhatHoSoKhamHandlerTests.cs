using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.HoSoKham.Commands.CapNhatHoSoKham;
using ClinicBooking.Application.UnitTests.Common;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace ClinicBooking.Application.UnitTests.Features.HoSoKham.Commands.CapNhatHoSoKham;

public sealed class CapNhatHoSoKhamHandlerTests
{
    [Fact]
    public async Task Handle_DungChuSoHuu_CapNhatThanhCong()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var now = DateTime.UtcNow;
        var duLieu = await HoSoKhamTestDataSeeder.TaoAsync(db, now);

        var hoSo = new ClinicBooking.Domain.Entities.HoSoKham
        {
            IdLichHen = duLieu.IdLichHen,
            IdBacSi = duLieu.IdBacSi,
            ChanDoan = "Cu",
            NgayKham = now,
            NgayTao = now
        };
        db.HoSoKham.Add(hoSo);
        await db.SaveChangesAsync();

        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.IdTaiKhoan.Returns(duLieu.IdTaiKhoanBacSi);

        var handler = new CapNhatHoSoKhamHandler(db, currentUser);
        await handler.Handle(
            new CapNhatHoSoKhamCommand(hoSo.IdHoSoKham, "Moi", "Ket qua moi", "Ghi chu moi"),
            CancellationToken.None);

        var entity = await db.HoSoKham.AsNoTracking().FirstAsync(x => x.IdHoSoKham == hoSo.IdHoSoKham);
        entity.ChanDoan.Should().Be("Moi");
    }

    [Fact]
    public async Task Handle_KhongDungChuSoHuu_ThrowForbiddenException()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var now = DateTime.UtcNow;
        var duLieu = await HoSoKhamTestDataSeeder.TaoAsync(db, now);

        var hoSo = new ClinicBooking.Domain.Entities.HoSoKham
        {
            IdLichHen = duLieu.IdLichHen,
            IdBacSi = duLieu.IdBacSi,
            ChanDoan = "Cu",
            NgayKham = now,
            NgayTao = now
        };
        db.HoSoKham.Add(hoSo);
        await db.SaveChangesAsync();

        var taiKhoanBacSiKhac = new ClinicBooking.Domain.Entities.TaiKhoan
        {
            TenDangNhap = "bacsi_khac",
            Email = "bacsi_khac@example.com",
            SoDienThoai = "0912345670",
            MatKhau = "hash",
            VaiTro = ClinicBooking.Domain.Enums.VaiTro.BacSi,
            TrangThai = true,
            NgayTao = now
        };
        db.TaiKhoan.Add(taiKhoanBacSiKhac);
        await db.SaveChangesAsync();

        var bacSiKhac = new ClinicBooking.Domain.Entities.BacSi
        {
            IdTaiKhoan = taiKhoanBacSiKhac.IdTaiKhoan,
            IdChuyenKhoa = db.ChuyenKhoa.Select(x => x.IdChuyenKhoa).First(),
            HoTen = "Bac Si Khac",
            LoaiHopDong = ClinicBooking.Domain.Enums.LoaiHopDong.HopDong,
            TrangThai = ClinicBooking.Domain.Enums.TrangThaiBacSi.DangLam,
            NgayTao = now
        };
        db.BacSi.Add(bacSiKhac);
        await db.SaveChangesAsync();

        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.IdTaiKhoan.Returns(taiKhoanBacSiKhac.IdTaiKhoan);

        var handler = new CapNhatHoSoKhamHandler(db, currentUser);
        var act = async () => await handler.Handle(
            new CapNhatHoSoKhamCommand(hoSo.IdHoSoKham, "Moi", null, null),
            CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>()
            .WithMessage("Ban khong co quyen cap nhat ho so kham nay.");
    }
}
