using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.ToaThuoc.Commands.HuyToaThuoc;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Enums;
using FluentAssertions;
using NSubstitute;

namespace ClinicBooking.Application.UnitTests.Features.ToaThuoc.Commands.HuyToaThuoc;

public class HuyToaThuocHandlerTests : IAsyncLifetime
{
    private TestDbContextFactory _dbContextFactory = null!;
    private ICurrentUserService _currentUserService = null!;

    public async Task InitializeAsync()
    {
        _dbContextFactory = new TestDbContextFactory();
        _currentUserService = Substitute.For<ICurrentUserService>();
        await Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        _dbContextFactory.Dispose();
        return Task.CompletedTask;
    }

    private static async Task<(ClinicBooking.Infrastructure.Persistence.AppDbContext ctx, int idBacSiTaiKhoan, int idBacSi, Domain.Entities.ToaThuoc toaThuoc, Domain.Entities.HoSoKham hoSoKham)>
        SeedAsync(TestDbContextFactory factory)
    {
        var ctx = factory.CreateContext();
        var now = DateTime.UtcNow;
        var seed = await HoSoKhamTestDataSeeder.TaoAsync(ctx, now);

        var thuoc = new Domain.Entities.Thuoc { TenThuoc = "Thuoc Test" };
        ctx.Thuoc.Add(thuoc);
        await ctx.SaveChangesAsync();

        var hoSoKham = new Domain.Entities.HoSoKham
        {
            IdLichHen = seed.IdLichHen,
            IdBacSi = seed.IdBacSi,
            NgayKham = now,
            NgayTao = now
        };
        ctx.HoSoKham.Add(hoSoKham);
        await ctx.SaveChangesAsync();

        var toaThuoc = new Domain.Entities.ToaThuoc
        {
            IdHoSoKham = hoSoKham.IdHoSoKham,
            IdThuoc = thuoc.IdThuoc
        };
        ctx.ToaThuoc.Add(toaThuoc);
        await ctx.SaveChangesAsync();

        return (ctx, seed.IdTaiKhoanBacSi, seed.IdBacSi, toaThuoc, hoSoKham);
    }

    [Fact]
    public async Task Handle_ValidInput_DeletesToaThuoc()
    {
        var (context, idBacSiTaiKhoan, _, toaThuoc, hoSoKham) = await SeedAsync(_dbContextFactory);

        _currentUserService.IdTaiKhoan.Returns(idBacSiTaiKhoan);
        _currentUserService.VaiTro.Returns(VaiTro.BacSi);

        var handler = new HuyToaThuocHandler(context, _currentUserService);
        var command = new HuyToaThuocCommand(hoSoKham.IdHoSoKham, toaThuoc.IdToaThuoc);

        await handler.Handle(command, CancellationToken.None);

        var deletedToa = context.ToaThuoc.FirstOrDefault(x => x.IdToaThuoc == toaThuoc.IdToaThuoc);
        deletedToa.Should().BeNull();
    }

    [Fact]
    public async Task Handle_ToaThuocNotFound_ThrowsNotFoundException()
    {
        var context = _dbContextFactory.CreateContext();
        _currentUserService.IdTaiKhoan.Returns(1);
        _currentUserService.VaiTro.Returns(VaiTro.BacSi);

        var handler = new HuyToaThuocHandler(context, _currentUserService);
        var command = new HuyToaThuocCommand(999, 999);

        await handler.Invoking(x => x.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_UnauthorizedBacSi_ThrowsForbiddenException()
    {
        var (context, _, _, toaThuoc, hoSoKham) = await SeedAsync(_dbContextFactory);

        _currentUserService.IdTaiKhoan.Returns(999);
        _currentUserService.VaiTro.Returns(VaiTro.BacSi);

        var handler = new HuyToaThuocHandler(context, _currentUserService);
        var command = new HuyToaThuocCommand(hoSoKham.IdHoSoKham, toaThuoc.IdToaThuoc);

        await handler.Invoking(x => x.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task Handle_AdminUser_CanDeleteAnyToa()
    {
        var (context, _, _, toaThuoc, hoSoKham) = await SeedAsync(_dbContextFactory);

        _currentUserService.IdTaiKhoan.Returns(1);
        _currentUserService.VaiTro.Returns(VaiTro.Admin);

        var handler = new HuyToaThuocHandler(context, _currentUserService);
        var command = new HuyToaThuocCommand(hoSoKham.IdHoSoKham, toaThuoc.IdToaThuoc);

        await handler.Handle(command, CancellationToken.None);

        var deletedToa = context.ToaThuoc.FirstOrDefault(x => x.IdToaThuoc == toaThuoc.IdToaThuoc);
        deletedToa.Should().BeNull();
    }
}
