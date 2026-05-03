using FluentValidation;

namespace ClinicBooking.Application.Features.Doctors.Queries.DanhSachBacSiCongKhai;

public sealed class DanhSachBacSiCongKhaiValidator : AbstractValidator<DanhSachBacSiCongKhaiQuery>
{
    public DanhSachBacSiCongKhaiValidator()
    {
        RuleFor(x => x.SoTrang).GreaterThanOrEqualTo(1);
        RuleFor(x => x.KichThuocTrang).InclusiveBetween(1, 100);
    }
}
