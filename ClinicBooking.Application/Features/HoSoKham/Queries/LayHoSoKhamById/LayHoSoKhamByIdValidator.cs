using FluentValidation;

namespace ClinicBooking.Application.Features.HoSoKham.Queries.LayHoSoKhamById;

public sealed class LayHoSoKhamByIdValidator : AbstractValidator<LayHoSoKhamByIdQuery>
{
    public LayHoSoKhamByIdValidator()
    {
        RuleFor(x => x.IdHoSoKham)
            .GreaterThan(0).WithMessage("Id ho so kham khong hop le.");
    }
}
