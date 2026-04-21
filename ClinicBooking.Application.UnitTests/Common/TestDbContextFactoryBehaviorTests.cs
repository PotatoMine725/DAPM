using ClinicBooking.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.UnitTests.Common;

public sealed class TestDbContextFactoryBehaviorTests
{
    [Fact]
    public async Task CreateContext_UsesSharedSqliteConnection_AndEnforcesConstraints()
    {
        using var factory = new TestDbContextFactory();

        using (var db1 = factory.CreateContext())
        {
            db1.Phong.Add(new Phong
            {
                MaPhong = "P-TEST-001",
                TenPhong = "Phong test",
                SucChua = 1,
                TrangThai = true
            });
            await db1.SaveChangesAsync();
        }

        using (var db2 = factory.CreateContext())
        {
            var exists = await db2.Phong.AnyAsync(x => x.MaPhong == "P-TEST-001");
            exists.Should().BeTrue();

            db2.Phong.Add(new Phong
            {
                MaPhong = "P-TEST-001",
                TenPhong = "Phong trung lap",
                SucChua = 2,
                TrangThai = true
            });

            var act = async () => await db2.SaveChangesAsync();

            await act.Should().ThrowAsync<DbUpdateException>();
        }
    }
}
