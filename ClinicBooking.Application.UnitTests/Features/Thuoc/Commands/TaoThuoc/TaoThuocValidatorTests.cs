using ClinicBooking.Application.Features.Thuoc.Commands.TaoThuoc;
using FluentAssertions;

namespace ClinicBooking.Application.UnitTests.Features.Thuoc.Commands.TaoThuoc;

public sealed class TaoThuocValidatorTests
{
    private readonly TaoThuocValidator _validator = new();

    [Fact]
    public void TenThuocRong_Invalid()
    {
        var result = _validator.Validate(new TaoThuocCommand(string.Empty, null, null, null));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void DuLieuHopLe_ValidateSuccess()
    {
        var result = _validator.Validate(new TaoThuocCommand("Paracetamol", "Paracetamol", "Vien", "Sau an"));
        result.IsValid.Should().BeTrue();
    }
}
