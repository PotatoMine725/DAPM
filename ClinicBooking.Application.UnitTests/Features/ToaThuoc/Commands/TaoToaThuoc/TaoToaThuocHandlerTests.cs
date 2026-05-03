using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.ToaThuoc.Commands.TaoToaThuoc;
using ClinicBooking.Application.Features.ToaThuoc.Dtos;
using ClinicBooking.Application.UnitTests.Common;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace ClinicBooking.Application.UnitTests.Features.ToaThuoc.Commands.TaoToaThuoc;

public sealed class TaoToaThuocHandlerTests
{
    [Fact]
    public async Task Handle_HopLe_TaoThanhCong()
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
        db.Thuoc.Add(new ClinicBooking.Domain.Entities.Thuoc { TenThuoc = "THUOC-UT-Paracetamol-500mg-20260422-Tao1" });
        db.Thuoc.Add(new ClinicBooking.Domain.Entities.Thuoc { TenThuoc = "THUOC-UT-Vitamin-C-20260422-Tao2" });
        await db.SaveChangesAsync();

        var dsThuoc = await db.Thuoc
            .OrderBy(x => x.IdThuoc)
            .Select(x => x.IdThuoc)
            .ToListAsync();

        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.IdTaiKhoan.Returns(duLieu.IdTaiKhoanBacSi);

        var handler = new TaoToaThuocHandler(db, currentUser);
        await handler.Handle(
            new TaoToaThuocCommand(
                hoSo.IdHoSoKham,
                new List<ToaThuocChiTietInput>
                {
                    new(dsThuoc[0], "1 vien", "Sau an", 5, null),
                    new(dsThuoc[1], "2 vien", "Truoc an", 3, "Toi")
                }),
            CancellationToken.None);

        var soDong = await db.ToaThuoc.CountAsync(x => x.IdHoSoKham == hoSo.IdHoSoKham);
        soDong.Should().Be(2);
    }

    [Fact]
    public async Task Handle_ThuocTrungLap_ThrowConflictException()
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
        db.Thuoc.Add(new ClinicBooking.Domain.Entities.Thuoc { TenThuoc = "THUOC-UT-Paracetamol-500mg-20260422-Conflict" });
        await db.SaveChangesAsync();
        var idThuoc = await db.Thuoc.Select(x => x.IdThuoc).FirstAsync();

        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.IdTaiKhoan.Returns(duLieu.IdTaiKhoanBacSi);

        var handler = new TaoToaThuocHandler(db, currentUser);
        var act = async () => await handler.Handle(
            new TaoToaThuocCommand(
                hoSo.IdHoSoKham,
                new List<ToaThuocChiTietInput>
                {
                    new(idThuoc, "1 vien", null, 5, null),
                    new(idThuoc, "2 vien", null, 3, null)
                }),
            CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("Don thuoc khong duoc co thuoc trung lap.");
    }
}
