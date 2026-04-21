using ClinicBooking.Application.Features.HoSoKham.Queries.LichSuKhamCuaToi;
using FluentAssertions;

namespace ClinicBooking.Application.UnitTests.Features.HoSoKham.Queries.LichSuKhamCuaToi;

public sealed class LichSuKhamCuaToiValidatorTests
{
    private readonly LichSuKhamCuaToiValidator _validator = new();

    [Fact]
    public void SoTrangKhongHopLe_Invalid()
    {
        var result = _validator.Validate(new LichSuKhamCuaToiQuery(0, 20));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void DuLieuHopLe_ValidateSuccess()
    {
        var result = _validator.Validate(new LichSuKhamCuaToiQuery(1, 20));
        result.IsValid.Should().BeTrue();
    }
}
