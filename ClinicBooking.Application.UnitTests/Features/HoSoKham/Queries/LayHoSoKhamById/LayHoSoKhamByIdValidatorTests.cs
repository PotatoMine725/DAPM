using ClinicBooking.Application.Features.HoSoKham.Queries.LayHoSoKhamById;
using FluentAssertions;

namespace ClinicBooking.Application.UnitTests.Features.HoSoKham.Queries.LayHoSoKhamById;

public sealed class LayHoSoKhamByIdValidatorTests
{
    private readonly LayHoSoKhamByIdValidator _validator = new();

    [Fact]
    public void IdKhongHopLe_Invalid()
    {
        var result = _validator.Validate(new LayHoSoKhamByIdQuery(0));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void IdHopLe_ValidateSuccess()
    {
        var result = _validator.Validate(new LayHoSoKhamByIdQuery(1));
        result.IsValid.Should().BeTrue();
    }
}
