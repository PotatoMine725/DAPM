using FluentValidation;

namespace ClinicBooking.Application.Features.BenhNhan.Queries.LayBenhNhanById;

public sealed class LayBenhNhanByIdValidator : AbstractValidator<LayBenhNhanByIdQuery>
{
    public LayBenhNhanByIdValidator()
    {
        RuleFor(x => x.IdBenhNhan)
            .GreaterThan(0).WithMessage("Id benh nhan khong hop le.");
    }
}
