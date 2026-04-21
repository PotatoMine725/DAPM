using ClinicBooking.Application.Abstractions.Notifications;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Common.Options;
using ClinicBooking.Application.Features.HangCho.Commands.GoiBenhNhanKeTiep;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Entities;
using ClinicBooking.Domain.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace ClinicBooking.Application.UnitTests.Features.HangCho.Commands.GoiBenhNhanKeTiep;

public sealed class GoiBenhNhanKeTiepHandlerTests
{
    private static IOptions<LichHenOptions> OptionsOf(bool tuDongHoanThanh) =>
        Microsoft.Extensions.Options.Options.Create(new LichHenOptions
        {
            HuyMuonTruocGio = 24,
            GiuChoThoiHanPhut = 15,
            MaLichHenPrefix = "LH",
            TuDongHoanThanhLuotHienTai = tuDongHoanThanh
        });

    [Fact]
    public async Task Handle_CoLuotCho_GoiLuotNhoNhat_ChuyenSangDangKham()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var bn = TestDataSeeder.SeedBenhNhan(db);
        var ca = TestDataSeeder.SeedCaLamViec(db);
        var lh1 = TestDataSeeder.SeedLichHen(db, bn.IdBenhNhan, ca.IdCaLamViec, soSlot: 1, trangThai: TrangThaiLichHen.DaXacNhan);
        var lh2 = TestDataSeeder.SeedLichHen(db, bn.IdBenhNhan, ca.IdCaLamViec, soSlot: 2, trangThai: TrangThaiLichHen.DaXacNhan);
        db.HangCho.AddRange(
            new ClinicBooking.Domain.Entities.HangCho { IdCaLamViec = ca.IdCaLamViec, IdLichHen = lh2.IdLichHen, SoThuTu = 2, TrangThai = TrangThaiHangCho.ChoKham, NgayCheckIn = DateTime.UtcNow },
            new ClinicBooking.Domain.Entities.HangCho { IdCaLamViec = ca.IdCaLamViec, IdLichHen = lh1.IdLichHen, SoThuTu = 1, TrangThai = TrangThaiHangCho.ChoKham, NgayCheckIn = DateTime.UtcNow });
        await db.SaveChangesAsync();

        var notif = Substitute.For<INotificationService>();
        var handler = new GoiBenhNhanKeTiepHandler(db, notif, OptionsOf(true));

        var result = await handler.Handle(new GoiBenhNhanKeTiepCommand(ca.IdCaLamViec), CancellationToken.None);

        result.SoThuTu.Should().Be(1);
        result.TrangThai.Should().Be(TrangThaiHangCho.DangKham);
        await notif.Received(1).GuiThongBaoGoiBenhNhanAsync(result.IdHangCho, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_TuDongHoanThanh_DongLuotHienTaiKhiGoiTiep()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var bn = TestDataSeeder.SeedBenhNhan(db);
        var ca = TestDataSeeder.SeedCaLamViec(db);
        var lhDang = TestDataSeeder.SeedLichHen(db, bn.IdBenhNhan, ca.IdCaLamViec, soSlot: 1, trangThai: TrangThaiLichHen.DangKham);
        var lhCho = TestDataSeeder.SeedLichHen(db, bn.IdBenhNhan, ca.IdCaLamViec, soSlot: 2, trangThai: TrangThaiLichHen.DaXacNhan);
        db.HangCho.AddRange(
            new ClinicBooking.Domain.Entities.HangCho { IdCaLamViec = ca.IdCaLamViec, IdLichHen = lhDang.IdLichHen, SoThuTu = 1, TrangThai = TrangThaiHangCho.DangKham, NgayCheckIn = DateTime.UtcNow },
            new ClinicBooking.Domain.Entities.HangCho { IdCaLamViec = ca.IdCaLamViec, IdLichHen = lhCho.IdLichHen, SoThuTu = 2, TrangThai = TrangThaiHangCho.ChoKham, NgayCheckIn = DateTime.UtcNow });
        await db.SaveChangesAsync();

        var handler = new GoiBenhNhanKeTiepHandler(db, Substitute.For<INotificationService>(), OptionsOf(true));
        var result = await handler.Handle(new GoiBenhNhanKeTiepCommand(ca.IdCaLamViec), CancellationToken.None);

        result.SoThuTu.Should().Be(2);
        (await db.LichHen.AsNoTracking().FirstAsync(x => x.IdLichHen == lhDang.IdLichHen)).TrangThai.Should().Be(TrangThaiLichHen.HoanThanh);
    }

    [Fact]
    public async Task Handle_KhongConLuotCho_ThrowNotFoundException()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var ca = TestDataSeeder.SeedCaLamViec(db);
        var handler = new GoiBenhNhanKeTiepHandler(db, Substitute.For<INotificationService>(), OptionsOf(false));

        var act = async () => await handler.Handle(new GoiBenhNhanKeTiepCommand(ca.IdCaLamViec), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
