using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.HoSoKham.Queries.LayHoSoKhamById;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Enums;
using FluentAssertions;
using NSubstitute;

namespace ClinicBooking.Application.UnitTests.Features.HoSoKham.Queries.LayHoSoKhamById;

public sealed class LayHoSoKhamByIdHandlerTests
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
            ChanDoan = "Chan doan A",
            NgayKham = now,
            NgayTao = now
        };
        db.HoSoKham.Add(hoSo);
        await db.SaveChangesAsync();

        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.VaiTro.Returns(VaiTro.BenhNhan);
        currentUser.IdTaiKhoan.Returns(duLieu.IdTaiKhoanBenhNhan);

        var handler = new LayHoSoKhamByIdHandler(db, currentUser);
        var result = await handler.Handle(new LayHoSoKhamByIdQuery(hoSo.IdHoSoKham), CancellationToken.None);

        result.IdHoSoKham.Should().Be(hoSo.IdHoSoKham);
        result.HoTenBenhNhan.Should().Be("Benh Nhan A");
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
            ChanDoan = "Chan doan A",
            NgayKham = now,
            NgayTao = now
        };
        db.HoSoKham.Add(hoSo);
        await db.SaveChangesAsync();

        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.VaiTro.Returns(VaiTro.BenhNhan);
        currentUser.IdTaiKhoan.Returns(duLieu.IdTaiKhoanBenhNhanKhac);

        var handler = new LayHoSoKhamByIdHandler(db, currentUser);
        var act = async () => await handler.Handle(new LayHoSoKhamByIdQuery(hoSo.IdHoSoKham), CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>()
            .WithMessage("Ban khong co quyen xem ho so kham nay.");
    }
}
