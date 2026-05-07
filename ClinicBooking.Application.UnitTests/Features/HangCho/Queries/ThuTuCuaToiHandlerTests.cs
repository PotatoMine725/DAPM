using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.HangCho.Queries.ThuTuCuaToi;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Enums;
using FluentAssertions;
using NSubstitute;
using HangChoEntity = ClinicBooking.Domain.Entities.HangCho;

namespace ClinicBooking.Application.UnitTests.Features.HangCho.Queries;

public sealed class ThuTuCuaToiHandlerTests
{
    private static ICurrentUserService TaoUser(int idTaiKhoan)
    {
        var user = Substitute.For<ICurrentUserService>();
        user.IdTaiKhoan.Returns(idTaiKhoan);
        user.VaiTro.Returns(VaiTro.BenhNhan);
        return user;
    }

    // -----------------------------------------------------------------------
    // Happy path: benh nhan da check-in vao hang cho cua ca => tra ve so thu tu
    // -----------------------------------------------------------------------
    [Fact]
    public async Task Handle_BenhNhanDaCheckIn_TraVeSoThuTu()
    {
        // Arrange
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var tk = TestDataSeeder.SeedTaiKhoan(db, VaiTro.BenhNhan);
        var bn = TestDataSeeder.SeedBenhNhan(db, idTaiKhoan: tk.IdTaiKhoan);
        var ca = TestDataSeeder.SeedCaLamViec(db);
        var lh = TestDataSeeder.SeedLichHen(db, bn.IdBenhNhan, ca.IdCaLamViec,
            trangThai: TrangThaiLichHen.DaXacNhan);

        var ngayCheckIn = new DateTime(2026, 5, 5, 8, 0, 0, DateTimeKind.Utc);
        db.HangCho.Add(new HangChoEntity
        {
            IdCaLamViec = ca.IdCaLamViec,
            IdLichHen = lh.IdLichHen,
            SoThuTu = 3,
            TrangThai = TrangThaiHangCho.ChoKham,
            NgayCheckIn = ngayCheckIn
        });
        db.SaveChanges();

        var user = TaoUser(tk.IdTaiKhoan);
        var handler = new ThuTuCuaToiHandler(db, user);

        // Act
        var result = await handler.Handle(new ThuTuCuaToiQuery(ca.IdCaLamViec), CancellationToken.None);

        // Assert
        result.DaCoTrongHangCho.Should().BeTrue();
        result.SoThuTu.Should().Be(3);
        result.IdBenhNhan.Should().Be(bn.IdBenhNhan);
        result.IdCaLamViec.Should().Be(ca.IdCaLamViec);
        result.NgayCheckIn.Should().Be(ngayCheckIn);
    }

    // -----------------------------------------------------------------------
    // Benh nhan chua check-in (khong co hang cho) => DaCoTrongHangCho = false
    // -----------------------------------------------------------------------
    [Fact]
    public async Task Handle_BenhNhanChuaCheckIn_TraVeKhongCoHangCho()
    {
        // Arrange
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var tk = TestDataSeeder.SeedTaiKhoan(db, VaiTro.BenhNhan);
        var bn = TestDataSeeder.SeedBenhNhan(db, idTaiKhoan: tk.IdTaiKhoan);
        var ca = TestDataSeeder.SeedCaLamViec(db);
        // Co lich hen nhung chua check-in => khong co ban ghi HangCho
        TestDataSeeder.SeedLichHen(db, bn.IdBenhNhan, ca.IdCaLamViec,
            trangThai: TrangThaiLichHen.ChoXacNhan);

        var user = TaoUser(tk.IdTaiKhoan);
        var handler = new ThuTuCuaToiHandler(db, user);

        // Act
        var result = await handler.Handle(new ThuTuCuaToiQuery(ca.IdCaLamViec), CancellationToken.None);

        // Assert
        result.DaCoTrongHangCho.Should().BeFalse();
        result.SoThuTu.Should().Be(0);
        result.NgayCheckIn.Should().BeNull();
    }

    // -----------------------------------------------------------------------
    // Tai khoan khong co ho so benh nhan => NotFoundException
    // -----------------------------------------------------------------------
    [Fact]
    public async Task Handle_TaiKhoanKhongCoHoSoBenhNhan_ThrowNotFound()
    {
        // Arrange
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        // Tao tai khoan BenhNhan nhung KHONG seed BenhNhan entity
        var tk = TestDataSeeder.SeedTaiKhoan(db, VaiTro.BenhNhan);
        var ca = TestDataSeeder.SeedCaLamViec(db);

        var user = TaoUser(tk.IdTaiKhoan);
        var handler = new ThuTuCuaToiHandler(db, user);

        // Act
        var act = async () => await handler.Handle(new ThuTuCuaToiQuery(ca.IdCaLamViec), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }
}
