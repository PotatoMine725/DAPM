using ClinicBooking.Application.Features.HoSoKham.Queries.LichSuKhamTheoBenhNhan;
using FluentAssertions;

namespace ClinicBooking.Application.UnitTests.Features.HoSoKham.Queries.LichSuKhamTheoBenhNhan;

public sealed class LichSuKhamTheoBenhNhanValidatorTests
{
    private readonly LichSuKhamTheoBenhNhanValidator _validator = new();

    [Fact]
    public void IdBenhNhanKhongHopLe_Invalid()
    {
        var result = _validator.Validate(new LichSuKhamTheoBenhNhanQuery(0, 1, 20));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void DuLieuHopLe_ValidateSuccess()
    {
        var result = _validator.Validate(new LichSuKhamTheoBenhNhanQuery(1, 1, 20));
        result.IsValid.Should().BeTrue();
    }
}
