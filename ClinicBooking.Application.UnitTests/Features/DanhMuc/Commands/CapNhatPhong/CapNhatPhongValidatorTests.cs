using ClinicBooking.Application.Features.DanhMuc.Commands.CapNhatPhong;
using FluentAssertions;

namespace ClinicBooking.Application.UnitTests.Features.DanhMuc.Commands.CapNhatPhong;

public sealed class CapNhatPhongValidatorTests
{
    private readonly CapNhatPhongValidator _validator = new();

    [Fact]
    public void IdKhongHopLe_Invalid()
    {
        var result = _validator.Validate(new CapNhatPhongCommand(0, "P1", "Ten", null, null, true));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void MaPhongRong_Invalid()
    {
        var result = _validator.Validate(new CapNhatPhongCommand(1, "", "Ten", null, null, true));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void DuLieuHopLe_ValidateSuccess()
    {
        var result = _validator.Validate(new CapNhatPhongCommand(1, "P1", "Ten", 20, "TB", true));
        result.IsValid.Should().BeTrue();
    }
}
