using ClinicBooking.Application.Features.HoSoKham.Commands.CapNhatHoSoKham;
using FluentAssertions;

namespace ClinicBooking.Application.UnitTests.Features.HoSoKham.Commands.CapNhatHoSoKham;

public sealed class CapNhatHoSoKhamValidatorTests
{
    private readonly CapNhatHoSoKhamValidator _validator = new();

    [Fact]
    public void IdKhongHopLe_Invalid()
    {
        var result = _validator.Validate(new CapNhatHoSoKhamCommand(0, null, null, null));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void DuLieuHopLe_ValidateSuccess()
    {
        var result = _validator.Validate(new CapNhatHoSoKhamCommand(1, "Chan doan", "Ket qua", "Ghi chu"));
        result.IsValid.Should().BeTrue();
    }
}
