using ClinicBooking.Application.Abstractions.Scheduling;
using ClinicBooking.Application.Features.Scheduling.Commands.SinhCaLamViecTuLichNoiTru;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Entities;
using ClinicBooking.Domain.Enums;
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
<<<<<<< HEAD
=======
        dinhNghiaSang.GioBatDauMacDinh = new TimeOnly(8, 0);
        dinhNghiaSang.GioKetThucMacDinh = new TimeOnly(12, 0);
        dinhNghiaChieu.GioBatDauMacDinh = new TimeOnly(13, 0);
        dinhNghiaChieu.GioKetThucMacDinh = new TimeOnly(17, 0);
        await db.SaveChangesAsync();
>>>>>>> 7e0dfb3 (feat_module2_finish_admin_scheduling_polish)

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
        conflictChecker.When(x => x.EnsureKhongXungDotAsync(default!, default!, default, default, default, default, default))
            .Do(ci =>
            {
                var ngay = (DateOnly)ci[2]!;
                var gioBatDau = (TimeOnly)ci[3]!;
                if (gioBatDau == new TimeOnly(13, 0) && ngay == DateOnly.FromDateTime(DateTime.UtcNow))
                {
                    throw new InvalidOperationException("Phong dang trung lich.");
                }
            });

        var handler = new SinhCaLamViecTuLichNoiTruHandler(db, conflictChecker);
        var result = await handler.Handle(new SinhCaLamViecTuLichNoiTruCommand(0), CancellationToken.None);

<<<<<<< HEAD
        result.SoCaSinh.Should().Be(1);
        result.SoCaBoQua.Should().Be(1);
        result.DanhSachXungDot.Should().ContainSingle(x => x.LyDo.Contains("trung lich", StringComparison.OrdinalIgnoreCase));
=======
        result.SoCaSinh.Should().Be(2);
        result.SoCaBoQua.Should().Be(0);
        result.DanhSachXungDot.Should().BeEmpty();
>>>>>>> 7e0dfb3 (feat_module2_finish_admin_scheduling_polish)

        (await db.CaLamViec.CountAsync()).Should().Be(2);
    }
}
