using ClinicBooking.Application.Abstractions.Scheduling;
using ClinicBooking.Application.Features.Scheduling.Commands.SinhCaLamViecTuLichNoiTru;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Entities;
using ClinicBooking.Domain.Enums;
using ClinicBooking.Infrastructure.Services.Scheduling;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace ClinicBooking.Application.UnitTests.Features.Scheduling.Commands.SinhCaLamViecTuLichNoiTru;

public sealed class SinhCaLamViecTuLichNoiTruHandlerTests
{
    [Fact]
    public async Task Handle_SinhCaVaBoQuaXungDot_TraKetQuaChiTiet()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var chuyenKhoa = TestDataSeeder.SeedChuyenKhoa(db, "CK NOITRU");
        var tk = TestDataSeeder.SeedTaiKhoan(db, VaiTro.BacSi);
        var phong = TestDataSeeder.SeedPhong(db);
        var dinhNghiaSang = TestDataSeeder.SeedDinhNghiaCa(db);
        var dinhNghiaChieu = TestDataSeeder.SeedDinhNghiaCa(db);

        var bacSi = new ClinicBooking.Domain.Entities.BacSi
        {
            IdTaiKhoan = tk.IdTaiKhoan,
            IdChuyenKhoa = chuyenKhoa.IdChuyenKhoa,
            HoTen = "BS Noi Tru",
            LoaiHopDong = LoaiHopDong.NoiTru,
            TrangThai = TrangThaiBacSi.DangLam,
            NgayTao = DateTime.UtcNow
        };
        db.BacSi.Add(bacSi);
        await db.SaveChangesAsync();

        db.LichNoiTru.AddRange(
            new LichNoiTru
            {
                IdBacSi = bacSi.IdBacSi,
                IdPhong = phong.IdPhong,
                IdDinhNghiaCa = dinhNghiaSang.IdDinhNghiaCa,
                NgayTrongTuan = (int)DateOnly.FromDateTime(DateTime.UtcNow).DayOfWeek,
                TrangThai = true
            },
            new LichNoiTru
            {
                IdBacSi = bacSi.IdBacSi,
                IdPhong = phong.IdPhong,
                IdDinhNghiaCa = dinhNghiaChieu.IdDinhNghiaCa,
                NgayTrongTuan = (int)DateOnly.FromDateTime(DateTime.UtcNow).DayOfWeek,
                TrangThai = true
            });
        await db.SaveChangesAsync();

        var conflictChecker = Substitute.For<ICaLamViecConflictChecker>();
        conflictChecker.EnsureKhongXungDotAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<DateOnly>(), Arg.Any<TimeOnly>(), Arg.Any<TimeOnly>(), Arg.Any<int?>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask)
            .AndDoes(ci =>
            {
                var ngay = ci.ArgAt<DateOnly>(2);
                var gioBatDau = ci.ArgAt<TimeOnly>(3);
                if (gioBatDau == new TimeOnly(13, 0) && ngay == DateOnly.FromDateTime(DateTime.UtcNow))
                {
                    throw new ClinicBooking.Application.Common.Exceptions.ConflictException("Phong dang trung lich.");
                }
            });

        var handler = new SinhCaLamViecTuLichNoiTruHandler(db, conflictChecker);
        var result = await handler.Handle(new SinhCaLamViecTuLichNoiTruCommand(0), CancellationToken.None);

        result.SoCaSinh.Should().Be(2);
        result.SoCaBoQua.Should().Be(0);
        result.DanhSachXungDot.Should().BeEmpty();

        (await db.CaLamViec.CountAsync()).Should().Be(2);
    }
}
