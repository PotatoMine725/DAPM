using ClinicBooking.Application.Features.DanhMuc.Commands.XoaDichVu;
using FluentAssertions;

namespace ClinicBooking.Application.UnitTests.Features.DanhMuc.Commands.XoaDichVu;

public sealed class XoaDichVuValidatorTests
{
    private readonly XoaDichVuValidator _validator = new();

    [Fact]
    public void IdKhongHopLe_Invalid()
    {
        var result = _validator.Validate(new XoaDichVuCommand(0));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void IdHopLe_ValidateSuccess()
    {
        var result = _validator.Validate(new XoaDichVuCommand(1));
        result.IsValid.Should().BeTrue();
    }
}
