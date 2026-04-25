using ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachDinhNghiaCa;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Entities;
using FluentAssertions;

namespace ClinicBooking.Application.UnitTests.Features.DanhMuc.Queries.DanhSachDinhNghiaCa;

public sealed class DanhSachDinhNghiaCaHandlerTests
{
    [Fact]
    public async Task Handle_CoBoLocTrangThaiVaTuKhoa_FilterDung()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        db.DinhNghiaCa.AddRange(
            new DinhNghiaCa
            {
                TenCa = "Ca sang thuong",
                GioBatDauMacDinh = new TimeOnly(7, 0),
                GioKetThucMacDinh = new TimeOnly(11, 0),
                TrangThai = true
            },
            new DinhNghiaCa
            {
                TenCa = "Ca toi",
                GioBatDauMacDinh = new TimeOnly(18, 0),
                GioKetThucMacDinh = new TimeOnly(22, 0),
                TrangThai = true
            },
            new DinhNghiaCa
            {
                TenCa = "Ca sang nghi",
                GioBatDauMacDinh = new TimeOnly(7, 0),
                GioKetThucMacDinh = new TimeOnly(11, 0),
                TrangThai = false
            });
        await db.SaveChangesAsync();

        var handler = new DanhSachDinhNghiaCaHandler(db);
        var result = await handler.Handle(new DanhSachDinhNghiaCaQuery(1, 20, true, "Ca sang"), CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].TenCa.Should().Be("Ca sang thuong");
    }

    [Fact]
    public async Task Handle_Paging_HoatDongDung()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        for (var i = 0; i < 5; i++)
        {
            db.DinhNghiaCa.Add(new DinhNghiaCa
            {
                TenCa = $"UT Ca {i + 1}",
                GioBatDauMacDinh = new TimeOnly(7 + i, 0),
                GioKetThucMacDinh = new TimeOnly(8 + i, 0),
                TrangThai = true
            });
        }

        await db.SaveChangesAsync();

        var handler = new DanhSachDinhNghiaCaHandler(db);
        var result = await handler.Handle(new DanhSachDinhNghiaCaQuery(2, 2, null, "UT Ca"), CancellationToken.None);

        result.Should().HaveCount(2);
        result[0].GioBatDauMacDinh.Should().Be(new TimeOnly(9, 0));
    }
}
