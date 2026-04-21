using ClinicBooking.Application.Features.BenhNhan.Queries.LayBenhNhanById;
using FluentAssertions;

namespace ClinicBooking.Application.UnitTests.Features.BenhNhan.Queries.LayBenhNhanById;

public sealed class LayBenhNhanByIdValidatorTests
{
    private readonly LayBenhNhanByIdValidator _validator = new();

    [Fact]
    public void IdKhongHopLe_Invalid()
    {
        var result = _validator.Validate(new LayBenhNhanByIdQuery(0));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void IdHopLe_ValidateSuccess()
    {
        var result = _validator.Validate(new LayBenhNhanByIdQuery(1));
        result.IsValid.Should().BeTrue();
    }
}
