using ClinicBooking.Application.Features.DanhMuc.Commands.CapNhatDichVu;
using FluentAssertions;

namespace ClinicBooking.Application.UnitTests.Features.DanhMuc.Commands.CapNhatDichVu;

public sealed class CapNhatDichVuValidatorTests
{
    private readonly CapNhatDichVuValidator _validator = new();

    [Fact]
    public void IdDichVuKhongHopLe_Invalid()
    {
        var result = _validator.Validate(new CapNhatDichVuCommand(0, 1, "DV", null, null, true));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void TenDichVuRong_Invalid()
    {
        var result = _validator.Validate(new CapNhatDichVuCommand(1, 1, "", null, null, true));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void DuLieuHopLe_ValidateSuccess()
    {
        var result = _validator.Validate(new CapNhatDichVuCommand(1, 1, "DV", "Mo ta", 30, true));
        result.IsValid.Should().BeTrue();
    }
}
