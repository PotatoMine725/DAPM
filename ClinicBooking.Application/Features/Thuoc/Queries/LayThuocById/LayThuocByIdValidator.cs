using FluentValidation;

namespace ClinicBooking.Application.Features.Thuoc.Queries.LayThuocById;

public sealed class LayThuocByIdValidator : AbstractValidator<LayThuocByIdQuery>
{
    public LayThuocByIdValidator()
    {
        RuleFor(x => x.IdThuoc)
            .GreaterThan(0).WithMessage("Id thuoc khong hop le.");
    }
}
