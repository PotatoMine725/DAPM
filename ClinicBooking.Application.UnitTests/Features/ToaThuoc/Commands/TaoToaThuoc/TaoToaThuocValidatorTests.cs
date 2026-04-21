using ClinicBooking.Application.Features.ToaThuoc.Commands.TaoToaThuoc;
using ClinicBooking.Application.Features.ToaThuoc.Dtos;
using FluentAssertions;

namespace ClinicBooking.Application.UnitTests.Features.ToaThuoc.Commands.TaoToaThuoc;

public sealed class TaoToaThuocValidatorTests
{
    private readonly TaoToaThuocValidator _validator = new();

    [Fact]
    public void DanhSachThuocRong_Invalid()
    {
        var result = _validator.Validate(new TaoToaThuocCommand(1, new List<ToaThuocChiTietInput>()));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void DuLieuHopLe_ValidateSuccess()
    {
        var result = _validator.Validate(
            new TaoToaThuocCommand(
                1,
                new List<ToaThuocChiTietInput> { new(1, "1 vien", "Sau an", 5, null) }));

        result.IsValid.Should().BeTrue();
    }
}
