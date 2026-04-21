using ClinicBooking.Application.Features.Thuoc.Commands.CapNhatThuoc;
using FluentAssertions;

namespace ClinicBooking.Application.UnitTests.Features.Thuoc.Commands.CapNhatThuoc;

public sealed class CapNhatThuocValidatorTests
{
    private readonly CapNhatThuocValidator _validator = new();

    [Fact]
    public void IdThuocKhongHopLe_Invalid()
    {
        var result = _validator.Validate(new CapNhatThuocCommand(0, "Paracetamol", null, null, null));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void DuLieuHopLe_ValidateSuccess()
    {
        var result = _validator.Validate(new CapNhatThuocCommand(1, "Paracetamol", "Paracetamol", "Vien", null));
        result.IsValid.Should().BeTrue();
    }
}
