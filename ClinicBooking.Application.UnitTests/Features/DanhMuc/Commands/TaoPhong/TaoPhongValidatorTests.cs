using ClinicBooking.Application.Features.DanhMuc.Commands.TaoPhong;
using FluentAssertions;

namespace ClinicBooking.Application.UnitTests.Features.DanhMuc.Commands.TaoPhong;

public sealed class TaoPhongValidatorTests
{
    private readonly TaoPhongValidator _validator = new();

    [Fact]
    public void MaPhongRong_Invalid()
    {
        var result = _validator.Validate(new TaoPhongCommand("", "Ten", null, null, true));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void SucChuaNgoaiKhoang_Invalid()
    {
        var result = _validator.Validate(new TaoPhongCommand("P101", "Ten", 0, null, true));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void DuLieuHopLe_ValidateSuccess()
    {
        var result = _validator.Validate(new TaoPhongCommand("P101", "Phong hop le", 25, "Trang bi", true));
        result.IsValid.Should().BeTrue();
    }
}
