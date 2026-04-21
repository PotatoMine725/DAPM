using ClinicBooking.Application.Features.HoSoKham.Commands.TaoHoSoKham;
using FluentAssertions;

namespace ClinicBooking.Application.UnitTests.Features.HoSoKham.Commands.TaoHoSoKham;

public sealed class TaoHoSoKhamValidatorTests
{
    private readonly TaoHoSoKhamValidator _validator = new();

    [Fact]
    public void IdLichHenKhongHopLe_Invalid()
    {
        var result = _validator.Validate(new TaoHoSoKhamCommand(0, null, null, null));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void DuLieuHopLe_ValidateSuccess()
    {
        var result = _validator.Validate(new TaoHoSoKhamCommand(1, "Chan doan", "Ket qua", "Ghi chu"));
        result.IsValid.Should().BeTrue();
    }
}
