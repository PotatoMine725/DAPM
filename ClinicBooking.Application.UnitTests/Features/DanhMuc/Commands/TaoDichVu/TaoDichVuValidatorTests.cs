using ClinicBooking.Application.Features.DanhMuc.Commands.TaoDichVu;
using FluentAssertions;

namespace ClinicBooking.Application.UnitTests.Features.DanhMuc.Commands.TaoDichVu;

public sealed class TaoDichVuValidatorTests
{
    private readonly TaoDichVuValidator _validator = new();

    [Fact]
    public void IdChuyenKhoaKhongHopLe_Invalid()
    {
        var result = _validator.Validate(new TaoDichVuCommand(0, "DV", null, null, true));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ThoiGianUocTinhNgoaiKhoang_Invalid()
    {
        var result = _validator.Validate(new TaoDichVuCommand(1, "DV", null, 0, true));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void DuLieuHopLe_ValidateSuccess()
    {
        var result = _validator.Validate(new TaoDichVuCommand(1, "DV", "Mo ta", 30, true));
        result.IsValid.Should().BeTrue();
    }
}
