using ClinicBooking.Application.Features.DanhMuc.Commands.XoaChuyenKhoa;
using FluentAssertions;

namespace ClinicBooking.Application.UnitTests.Features.DanhMuc.Commands.XoaChuyenKhoa;

public sealed class XoaChuyenKhoaValidatorTests
{
    private readonly XoaChuyenKhoaValidator _validator = new();

    [Fact]
    public void IdKhongHopLe_Invalid()
    {
        var result = _validator.Validate(new XoaChuyenKhoaCommand(0));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void IdHopLe_ValidateSuccess()
    {
        var result = _validator.Validate(new XoaChuyenKhoaCommand(1));
        result.IsValid.Should().BeTrue();
    }
}
