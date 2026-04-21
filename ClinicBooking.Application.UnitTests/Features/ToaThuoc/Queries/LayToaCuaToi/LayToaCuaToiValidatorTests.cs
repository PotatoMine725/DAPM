using ClinicBooking.Application.Features.ToaThuoc.Queries.LayToaCuaToi;
using FluentAssertions;

namespace ClinicBooking.Application.UnitTests.Features.ToaThuoc.Queries.LayToaCuaToi;

public sealed class LayToaCuaToiValidatorTests
{
    private readonly LayToaCuaToiValidator _validator = new();

    [Fact]
    public void SoTrangKhongHopLe_Invalid()
    {
        var result = _validator.Validate(new LayToaCuaToiQuery(0, 20));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void DuLieuHopLe_ValidateSuccess()
    {
        var result = _validator.Validate(new LayToaCuaToiQuery(1, 20));
        result.IsValid.Should().BeTrue();
    }
}
