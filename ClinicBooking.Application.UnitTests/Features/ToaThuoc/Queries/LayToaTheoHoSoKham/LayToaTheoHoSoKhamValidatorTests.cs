using ClinicBooking.Application.Features.ToaThuoc.Queries.LayToaTheoHoSoKham;
using FluentAssertions;

namespace ClinicBooking.Application.UnitTests.Features.ToaThuoc.Queries.LayToaTheoHoSoKham;

public sealed class LayToaTheoHoSoKhamValidatorTests
{
    private readonly LayToaTheoHoSoKhamValidator _validator = new();

    [Fact]
    public void IdKhongHopLe_Invalid()
    {
        var result = _validator.Validate(new LayToaTheoHoSoKhamQuery(0));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void IdHopLe_ValidateSuccess()
    {
        var result = _validator.Validate(new LayToaTheoHoSoKhamQuery(1));
        result.IsValid.Should().BeTrue();
    }
}
