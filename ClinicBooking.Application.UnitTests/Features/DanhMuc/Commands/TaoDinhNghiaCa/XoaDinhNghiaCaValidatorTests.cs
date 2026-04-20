using ClinicBooking.Application.Features.DanhMuc.Commands.XoaDinhNghiaCa;
using FluentAssertions;

namespace ClinicBooking.Application.UnitTests.Features.DanhMuc.Commands.TaoDinhNghiaCa;

public sealed class XoaDinhNghiaCaValidatorTests
{
    private readonly XoaDinhNghiaCaValidator _validator = new();

    [Fact]
    public void IdKhongHopLe_Invalid()
    {
        var result = _validator.Validate(new XoaDinhNghiaCaCommand(0));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void IdHopLe_ValidateSuccess()
    {
        var result = _validator.Validate(new XoaDinhNghiaCaCommand(1));
        result.IsValid.Should().BeTrue();
    }
}
