using ClinicBooking.Application.Features.DanhMuc.Commands.CapNhatDinhNghiaCa;
using FluentAssertions;

namespace ClinicBooking.Application.UnitTests.Features.DanhMuc.Commands.TaoDinhNghiaCa;

public sealed class CapNhatDinhNghiaCaValidatorTests
{
    private readonly CapNhatDinhNghiaCaValidator _validator = new();

    [Fact]
    public void IdKhongHopLe_Invalid()
    {
        var result = _validator.Validate(
            new CapNhatDinhNghiaCaCommand(
                0,
                "Ca sang",
                new TimeOnly(7, 0),
                new TimeOnly(11, 0),
                null,
                true));

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void GioBatDauSauGioKetThuc_Invalid()
    {
        var result = _validator.Validate(
            new CapNhatDinhNghiaCaCommand(
                1,
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
            new CapNhatDinhNghiaCaCommand(
                1,
                "Ca chieu",
                new TimeOnly(13, 0),
                new TimeOnly(17, 0),
                "Mo ta",
                true));

        result.IsValid.Should().BeTrue();
    }
}
