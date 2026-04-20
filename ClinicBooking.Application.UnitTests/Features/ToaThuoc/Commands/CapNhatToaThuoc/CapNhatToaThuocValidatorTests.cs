using ClinicBooking.Application.Features.ToaThuoc.Commands.CapNhatToaThuoc;
using ClinicBooking.Application.Features.ToaThuoc.Dtos;
using FluentAssertions;

namespace ClinicBooking.Application.UnitTests.Features.ToaThuoc.Commands.CapNhatToaThuoc;

public sealed class CapNhatToaThuocValidatorTests
{
    private readonly CapNhatToaThuocValidator _validator = new();

    [Fact]
    public void IdHoSoKhamKhongHopLe_Invalid()
    {
        var result = _validator.Validate(new CapNhatToaThuocCommand(0, new List<ToaThuocChiTietInput>()));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void DuLieuHopLe_ValidateSuccess()
    {
        var result = _validator.Validate(
            new CapNhatToaThuocCommand(1, new List<ToaThuocChiTietInput> { new(1, "1 vien", null, 5, null) }));
        result.IsValid.Should().BeTrue();
    }
}
