using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.ToaThuoc.Queries.LayToaTheoHoSoKham;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Enums;
using FluentAssertions;
using NSubstitute;

namespace ClinicBooking.Application.UnitTests.Features.ToaThuoc.Queries.LayToaTheoHoSoKham;

public sealed class LayToaTheoHoSoKhamHandlerTests
{
    [Fact]
    public async Task Handle_BenhNhanChuSoHuu_XemDuoc()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var now = DateTime.UtcNow;
        var duLieu = await HoSoKhamTestDataSeeder.TaoAsync(db, now);

        var hoSo = new ClinicBooking.Domain.Entities.HoSoKham
        {
            IdLichHen = duLieu.IdLichHen,
            IdBacSi = duLieu.IdBacSi,
            NgayKham = now,
            NgayTao = now
        };
        db.HoSoKham.Add(hoSo);
        db.Thuoc.Add(new ClinicBooking.Domain.Entities.Thuoc { TenThuoc = "Paracetamol 500mg" });
        await db.SaveChangesAsync();
        var idThuoc = db.Thuoc.Select(x => x.IdThuoc).First();

        db.ToaThuoc.Add(new ClinicBooking.Domain.Entities.ToaThuoc
        {
            IdHoSoKham = hoSo.IdHoSoKham,
            IdThuoc = idThuoc,
            LieuLuong = "1 vien"
        });
        await db.SaveChangesAsync();

        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.VaiTro.Returns(VaiTro.BenhNhan);
        currentUser.IdTaiKhoan.Returns(duLieu.IdTaiKhoanBenhNhan);

        var handler = new LayToaTheoHoSoKhamHandler(db, currentUser);
        var result = await handler.Handle(new LayToaTheoHoSoKhamQuery(hoSo.IdHoSoKham), CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].TenThuoc.Should().Be("Paracetamol 500mg");
    }

    [Fact]
    public async Task Handle_BenhNhanKhongPhaiChuSoHuu_ThrowForbiddenException()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();
        var now = DateTime.UtcNow;
        var duLieu = await HoSoKhamTestDataSeeder.TaoAsync(db, now);

        var hoSo = new ClinicBooking.Domain.Entities.HoSoKham
        {
            IdLichHen = duLieu.IdLichHen,
            IdBacSi = duLieu.IdBacSi,
            NgayKham = now,
            NgayTao = now
        };
        db.HoSoKham.Add(hoSo);
        await db.SaveChangesAsync();

        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.VaiTro.Returns(VaiTro.BenhNhan);
        currentUser.IdTaiKhoan.Returns(duLieu.IdTaiKhoanBenhNhanKhac);

        var handler = new LayToaTheoHoSoKhamHandler(db, currentUser);
        var act = async () => await handler.Handle(new LayToaTheoHoSoKhamQuery(hoSo.IdHoSoKham), CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>()
            .WithMessage("Ban khong co quyen xem toa thuoc nay.");
    }
}
