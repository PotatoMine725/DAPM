using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Features.ToaThuoc.Commands.CapNhatToaThuoc;
using ClinicBooking.Application.Features.ToaThuoc.Dtos;
using ClinicBooking.Application.UnitTests.Common;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace ClinicBooking.Application.UnitTests.Features.ToaThuoc.Commands.CapNhatToaThuoc;

public sealed class CapNhatToaThuocHandlerTests
{
    [Fact]
    public async Task Handle_HopLe_ReplaceDanhSachToa()
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
        var thuoc1 = new ClinicBooking.Domain.Entities.Thuoc { TenThuoc = "THUOC-UT-CapNhatToa-20260422-1" };
        var thuoc2 = new ClinicBooking.Domain.Entities.Thuoc { TenThuoc = "THUOC-UT-CapNhatToa-20260422-2" };
        db.Thuoc.AddRange(thuoc1, thuoc2);
        await db.SaveChangesAsync();

        var dsThuoc = new List<int> { thuoc1.IdThuoc, thuoc2.IdThuoc };

        db.ToaThuoc.Add(new ClinicBooking.Domain.Entities.ToaThuoc
        {
            IdHoSoKham = hoSo.IdHoSoKham,
            IdThuoc = dsThuoc[0],
            LieuLuong = "Cu"
        });
        await db.SaveChangesAsync();

        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.IdTaiKhoan.Returns(duLieu.IdTaiKhoanBacSi);

        var handler = new CapNhatToaThuocHandler(db, currentUser);
        await handler.Handle(
            new CapNhatToaThuocCommand(
                hoSo.IdHoSoKham,
                new List<ToaThuocChiTietInput>
                {
                    new(dsThuoc[1], "Moi", "Sau an", 7, null)
                }),
            CancellationToken.None);

        var toa = await db.ToaThuoc.AsNoTracking().Where(x => x.IdHoSoKham == hoSo.IdHoSoKham).ToListAsync();
        toa.Should().HaveCount(1);
        toa[0].IdThuoc.Should().Be(dsThuoc[1]);
    }
}
