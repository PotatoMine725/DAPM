using ClinicBooking.Application.Features.BenhNhan.Commands.TaoBenhNhanWalkIn;
using ClinicBooking.Domain.Enums;
using FluentAssertions;

namespace ClinicBooking.Application.UnitTests.Features.BenhNhan.Commands.TaoBenhNhanWalkIn;

public sealed class TaoBenhNhanWalkInValidatorTests
{
    private readonly TaoBenhNhanWalkInValidator _validator = new();

    [Fact]
    public void SoDienThoaiKhongHopLe_Invalid()
    {
        var result = _validator.Validate(new TaoBenhNhanWalkInCommand("A", "123", null, null, null, null));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void DuLieuHopLe_ValidateSuccess()
    {
        var result = _validator.Validate(
            new TaoBenhNhanWalkInCommand("A", "0912345678", new DateOnly(1995, 1, 1), GioiTinh.Nam, "123456789012", null));
        result.IsValid.Should().BeTrue();
    }
}
