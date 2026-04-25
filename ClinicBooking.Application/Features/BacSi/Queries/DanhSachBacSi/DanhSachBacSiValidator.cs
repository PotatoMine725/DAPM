using FluentValidation;

namespace ClinicBooking.Application.Features.BacSi.Queries.DanhSachBacSi;

public sealed class DanhSachBacSiValidator : AbstractValidator<DanhSachBacSiQuery>
{
    public DanhSachBacSiValidator()
    {
        RuleFor(x => x.SoTrang)
            .GreaterThanOrEqualTo(1).WithMessage("So trang phai lon hon hoac bang 1.");

        RuleFor(x => x.KichThuocTrang)
            .InclusiveBetween(1, 100).WithMessage("Kich thuoc trang phai trong khoang 1-100.");
    }
}
