using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.ToaThuoc.Commands.HuyToaThuoc;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Entities;
using ClinicBooking.Domain.Enums;
using NSubstitute;

namespace ClinicBooking.Application.UnitTests.Features.ToaThuoc.Commands.HuyToaThuoc;

public class HuyToaThuocHandlerTests : IAsyncLifetime
{
    private TestDbContextFactory _dbContextFactory = null!;
    private HuyToaThuocHandler _handler = null!;
    private ICurrentUserService _currentUserService = null!;

    public async Task InitializeAsync()
    {
        _dbContextFactory = new TestDbContextFactory();
        _currentUserService = Substitute.For<ICurrentUserService>();
        var context = _dbContextFactory.CreateDbContext();
        _handler = new HuyToaThuocHandler(context, _currentUserService);
    }

    public async Task DisposeAsync()
    {
        _dbContextFactory.Dispose();
    }

    [Fact]
    public async Task Handle_ValidInput_DeletesToaThuoc()
    {
        // Arrange
        var context = _dbContextFactory.CreateDbContext();
        var seeder = new HoSoKhamTestDataSeeder();
        seeder.SeedTestData(context);
        await context.SaveChangesAsync();

        var toaThuoc = context.ToaThuoc.First();
        var hoSoKham = context.HoSoKham.First();
        var bacSi = context.BacSi.First();

        _currentUserService.IdTaiKhoan.Returns(bacSi.IdTaiKhoan);
        _currentUserService.VaiTro.Returns(VaiTro.BacSi);

        var handler = new HuyToaThuocHandler(context, _currentUserService);
        var command = new HuyToaThuocCommand(hoSoKham.IdHoSoKham, toaThuoc.IdToaThuoc);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedToa = context.ToaThuoc.FirstOrDefault(x => x.IdToaThuoc == toaThuoc.IdToaThuoc);
        deletedToa.Should().BeNull(); // Hard-delete
    }

    [Fact]
    public async Task Handle_ToaThuocNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var context = _dbContextFactory.CreateDbContext();
        _currentUserService.IdTaiKhoan.Returns(1);
        _currentUserService.VaiTro.Returns(VaiTro.BacSi);

        var handler = new HuyToaThuocHandler(context, _currentUserService);
        var command = new HuyToaThuocCommand(999, 999);

        // Act & Assert
        await handler.Invoking(x => x.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_UnauthorizedBacSi_ThrowsForbiddenException()
    {
        // Arrange
        var context = _dbContextFactory.CreateDbContext();
        var seeder = new HoSoKhamTestDataSeeder();
        seeder.SeedTestData(context);
        await context.SaveChangesAsync();

        var toaThuoc = context.ToaThuoc.First();
        var hoSoKham = context.HoSoKham.First();

        // Khác bác sĩ tạo hồ sơ
        _currentUserService.IdTaiKhoan.Returns(999);
        _currentUserService.VaiTro.Returns(VaiTro.BacSi);

        var handler = new HuyToaThuocHandler(context, _currentUserService);
        var command = new HuyToaThuocCommand(hoSoKham.IdHoSoKham, toaThuoc.IdToaThuoc);

        // Act & Assert
        await handler.Invoking(x => x.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task Handle_AdminUser_CanDeleteAnyToa()
    {
        // Arrange
        var context = _dbContextFactory.CreateDbContext();
        var seeder = new HoSoKhamTestDataSeeder();
        seeder.SeedTestData(context);
        await context.SaveChangesAsync();

        var toaThuoc = context.ToaThuoc.First();

        // Admin có quyền
        _currentUserService.IdTaiKhoan.Returns(1);
        _currentUserService.VaiTro.Returns(VaiTro.Admin);

        var handler = new HuyToaThuocHandler(context, _currentUserService);
        var command = new HuyToaThuocCommand(toaThuoc.IdHoSoKham, toaThuoc.IdToaThuoc);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedToa = context.ToaThuoc.FirstOrDefault(x => x.IdToaThuoc == toaThuoc.IdToaThuoc);
        deletedToa.Should().BeNull(); // Hard-delete
    }
}
