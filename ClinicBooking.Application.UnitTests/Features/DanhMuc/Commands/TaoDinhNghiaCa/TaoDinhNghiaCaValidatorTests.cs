using ClinicBooking.Application.Features.DanhMuc.Commands.TaoDinhNghiaCa;
using FluentAssertions;

namespace ClinicBooking.Application.UnitTests.Features.DanhMuc.Commands.TaoDinhNghiaCa;

public sealed class TaoDinhNghiaCaValidatorTests
{
    private readonly TaoDinhNghiaCaValidator _validator = new();

    [Fact]
    public void TenCaRong_Invalid()
    {
        var result = _validator.Validate(
            new TaoDinhNghiaCaCommand(
                string.Empty,
                new TimeOnly(8, 0),
                new TimeOnly(12, 0),
                null,
                true));

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void GioBatDauSauGioKetThuc_Invalid()
    {
        var result = _validator.Validate(
            new TaoDinhNghiaCaCommand(
                "Ca toi",
                new TimeOnly(18, 0),
                new TimeOnly(17, 0),
                null,
                true));

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void DuLieuHopLe_ValidateSuccess()
    {
        var result = _validator.Validate(
            new TaoDinhNghiaCaCommand(
                "Ca sang",
                new TimeOnly(7, 0),
                new TimeOnly(11, 30),
                "Mo ta",
                true));

        result.IsValid.Should().BeTrue();
    }
}
