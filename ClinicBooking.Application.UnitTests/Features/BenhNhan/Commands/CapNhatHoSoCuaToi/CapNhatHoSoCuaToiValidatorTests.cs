using ClinicBooking.Application.Features.BenhNhan.Commands.CapNhatHoSoCuaToi;
using ClinicBooking.Domain.Enums;
using FluentAssertions;

namespace ClinicBooking.Application.UnitTests.Features.BenhNhan.Commands.CapNhatHoSoCuaToi;

public sealed class CapNhatHoSoCuaToiValidatorTests
{
    private readonly CapNhatHoSoCuaToiValidator _validator = new();

    [Fact]
    public void HoTenRong_Invalid()
    {
        var result = _validator.Validate(new CapNhatHoSoCuaToiCommand("", null, null, null, null));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void DuLieuHopLe_ValidateSuccess()
    {
        var result = _validator.Validate(
            new CapNhatHoSoCuaToiCommand("Nguyen Van A", new DateOnly(1995, 1, 1), GioiTinh.Nam, "123456789012", "Dia chi"));
        result.IsValid.Should().BeTrue();
    }
}
