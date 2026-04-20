using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.HoSoKham.Commands.TaoHoSoKham;
using ClinicBooking.Application.UnitTests.Common;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace ClinicBooking.Application.UnitTests.Features.HoSoKham.Commands.TaoHoSoKham;

public sealed class TaoHoSoKhamHandlerTests
{
    [Fact]
    public async Task Handle_HopLe_TaoThanhCong()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var now = new DateTime(2026, 4, 21, 7, 0, 0, DateTimeKind.Utc);
        var duLieu = await HoSoKhamTestDataSeeder.TaoAsync(db, now);

        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.IdTaiKhoan.Returns(duLieu.IdTaiKhoanBacSi);

        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider.UtcNow.Returns(now);

        var handler = new TaoHoSoKhamHandler(db, currentUser, dateTimeProvider);
        var id = await handler.Handle(
            new TaoHoSoKhamCommand(duLieu.IdLichHen, "Chan doan A", "Ket qua A", "Ghi chu A"),
            CancellationToken.None);

        id.Should().BeGreaterThan(0);
        var hoSo = await db.HoSoKham.AsNoTracking().FirstAsync(x => x.IdHoSoKham == id);
        hoSo.IdBacSi.Should().Be(duLieu.IdBacSi);
    }

    [Fact]
    public async Task Handle_BacSiKhongThuocCa_ThrowForbiddenException()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var now = DateTime.UtcNow;
        var duLieu = await HoSoKhamTestDataSeeder.TaoAsync(db, now);

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

        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider.UtcNow.Returns(now);

        var handler = new TaoHoSoKhamHandler(db, currentUser, dateTimeProvider);
        var act = async () => await handler.Handle(
            new TaoHoSoKhamCommand(duLieu.IdLichHen, null, null, null),
            CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>()
            .WithMessage("Ban khong co quyen tao ho so cho lich hen nay.");
    }
}
