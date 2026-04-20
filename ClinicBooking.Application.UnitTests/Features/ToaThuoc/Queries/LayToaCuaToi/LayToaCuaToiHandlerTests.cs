using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Features.ToaThuoc.Queries.LayToaCuaToi;
using ClinicBooking.Application.UnitTests.Common;
using FluentAssertions;
using NSubstitute;

namespace ClinicBooking.Application.UnitTests.Features.ToaThuoc.Queries.LayToaCuaToi;

public sealed class LayToaCuaToiHandlerTests
{
    [Fact]
    public async Task Handle_HopLe_TraVeDanhSach()
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
        currentUser.IdTaiKhoan.Returns(duLieu.IdTaiKhoanBenhNhan);

        var handler = new LayToaCuaToiHandler(db, currentUser);
        var result = await handler.Handle(new LayToaCuaToiQuery(1, 20), CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].TenThuoc.Should().Be("Paracetamol 500mg");
    }
}
