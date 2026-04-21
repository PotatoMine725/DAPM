using ClinicBooking.Application.Features.DanhMuc.Commands.CapNhatChuyenKhoa;
using FluentAssertions;

namespace ClinicBooking.Application.UnitTests.Features.DanhMuc.Commands.CapNhatChuyenKhoa;

public sealed class CapNhatChuyenKhoaValidatorTests
{
    private readonly CapNhatChuyenKhoaValidator _validator = new();

    [Fact]
    public void IdChuyenKhoaKhongHopLe_Invalid()
    {
        var result = _validator.Validate(new CapNhatChuyenKhoaCommand(0, "Noi", null, 20, null, null, true));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void TenChuyenKhoaRong_Invalid()
    {
        var result = _validator.Validate(new CapNhatChuyenKhoaCommand(1, "", null, 20, null, null, true));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void GioMoSauGioDong_Invalid()
    {
        var result = _validator.Validate(
            new CapNhatChuyenKhoaCommand(
                1,
                "Noi",
                null,
                20,
                new TimeOnly(18, 0),
                new TimeOnly(8, 0),
                true));

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void DuLieuHopLe_ValidateSuccess()
    {
        var result = _validator.Validate(
            new CapNhatChuyenKhoaCommand(
                1,
                "Noi Tim Mach",
                "Mo ta",
                15,
                new TimeOnly(7, 0),
                new TimeOnly(17, 0),
                true));

        result.IsValid.Should().BeTrue();
    }
}
