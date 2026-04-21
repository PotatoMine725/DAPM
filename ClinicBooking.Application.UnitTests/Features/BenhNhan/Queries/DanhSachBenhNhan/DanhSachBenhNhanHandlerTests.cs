using ClinicBooking.Application.Features.BenhNhan.Queries.DanhSachBenhNhan;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Enums;
using FluentAssertions;

namespace ClinicBooking.Application.UnitTests.Features.BenhNhan.Queries.DanhSachBenhNhan;

public sealed class DanhSachBenhNhanHandlerTests
{
    [Fact]
    public async Task Handle_CoTuKhoa_FilterDung()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var taiKhoan1 = new ClinicBooking.Domain.Entities.TaiKhoan
        {
            TenDangNhap = "a",
            Email = "a@example.com",
            SoDienThoai = "0912345678",
            MatKhau = "hash",
            VaiTro = VaiTro.BenhNhan,
            TrangThai = true,
            NgayTao = DateTime.UtcNow
        };
        var taiKhoan2 = new ClinicBooking.Domain.Entities.TaiKhoan
        {
            TenDangNhap = "b",
            Email = "b@example.com",
            SoDienThoai = "0912345679",
            MatKhau = "hash",
            VaiTro = VaiTro.BenhNhan,
            TrangThai = true,
            NgayTao = DateTime.UtcNow
        };
        db.TaiKhoan.AddRange(taiKhoan1, taiKhoan2);
        await db.SaveChangesAsync();

        db.BenhNhan.Add(new ClinicBooking.Domain.Entities.BenhNhan
        {
            IdTaiKhoan = taiKhoan1.IdTaiKhoan,
            HoTen = "Nguyen Van A",
            NgayTao = DateTime.UtcNow
        });
        db.BenhNhan.Add(new ClinicBooking.Domain.Entities.BenhNhan
        {
            IdTaiKhoan = taiKhoan2.IdTaiKhoan,
            HoTen = "Tran Thi B",
            NgayTao = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        var handler = new DanhSachBenhNhanHandler(db);
        var result = await handler.Handle(new DanhSachBenhNhanQuery(1, 20, null, "0912345678"), CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].HoTen.Should().Be("Nguyen Van A");
    }

    [Fact]
    public async Task Handle_FilterBiHanChe_HoatDongDung()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var taiKhoan1 = new ClinicBooking.Domain.Entities.TaiKhoan
        {
            TenDangNhap = "a",
            Email = "a@example.com",
            SoDienThoai = "0912345678",
            MatKhau = "hash",
            VaiTro = VaiTro.BenhNhan,
            TrangThai = true,
            NgayTao = DateTime.UtcNow
        };
        var taiKhoan2 = new ClinicBooking.Domain.Entities.TaiKhoan
        {
            TenDangNhap = "b",
            Email = "b@example.com",
            SoDienThoai = "0912345679",
            MatKhau = "hash",
            VaiTro = VaiTro.BenhNhan,
            TrangThai = true,
            NgayTao = DateTime.UtcNow
        };
        db.TaiKhoan.AddRange(taiKhoan1, taiKhoan2);
        await db.SaveChangesAsync();

        db.BenhNhan.Add(new ClinicBooking.Domain.Entities.BenhNhan
        {
            IdTaiKhoan = taiKhoan1.IdTaiKhoan,
            HoTen = "A",
            BiHanChe = true,
            NgayTao = DateTime.UtcNow
        });
        db.BenhNhan.Add(new ClinicBooking.Domain.Entities.BenhNhan
        {
            IdTaiKhoan = taiKhoan2.IdTaiKhoan,
            HoTen = "B",
            BiHanChe = false,
            NgayTao = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        var handler = new DanhSachBenhNhanHandler(db);
        var result = await handler.Handle(new DanhSachBenhNhanQuery(1, 20, true, null), CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].BiHanChe.Should().BeTrue();
    }
}
