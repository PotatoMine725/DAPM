using ClinicBooking.Application.Features.BenhNhan.Queries.DanhSachBenhNhan;
using FluentAssertions;

namespace ClinicBooking.Application.UnitTests.Features.BenhNhan.Queries.DanhSachBenhNhan;

public sealed class DanhSachBenhNhanValidatorTests
{
    private readonly DanhSachBenhNhanValidator _validator = new();

    [Fact]
    public void SoTrangKhongHopLe_Invalid()
    {
        var result = _validator.Validate(new DanhSachBenhNhanQuery(0, 20, null, null));
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void DuLieuHopLe_ValidateSuccess()
    {
        var result = _validator.Validate(new DanhSachBenhNhanQuery(1, 20, null, "A"));
        result.IsValid.Should().BeTrue();
    }
}
