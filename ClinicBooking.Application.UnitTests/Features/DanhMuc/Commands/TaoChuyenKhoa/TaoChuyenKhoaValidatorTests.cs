using ClinicBooking.Application.Features.DanhMuc.Commands.TaoChuyenKhoa;
using FluentAssertions;

namespace ClinicBooking.Application.UnitTests.Features.DanhMuc.Commands.TaoChuyenKhoa;

public sealed class TaoChuyenKhoaValidatorTests
{
    private readonly TaoChuyenKhoaValidator _validator = new();

    [Fact]
    public void TenRong_Invalid()
    {
        var result = _validator.Validate(
            new TaoChuyenKhoaCommand("", null, 10, null, null, true));

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void GioMoSauGioDong_Invalid()
    {
        var result = _validator.Validate(
            new TaoChuyenKhoaCommand(
                "Ten",
                null,
                10,
                new TimeOnly(18, 0),
                new TimeOnly(8, 0),
                true));

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void HopLe_KhiChiCoMotGioHoacGioMoTruocDong()
    {
        var result = _validator.Validate(
            new TaoChuyenKhoaCommand(
                "Ten hop le",
                null,
                20,
                new TimeOnly(7, 0),
                new TimeOnly(17, 0),
                true));

        result.IsValid.Should().BeTrue();
    }
}
