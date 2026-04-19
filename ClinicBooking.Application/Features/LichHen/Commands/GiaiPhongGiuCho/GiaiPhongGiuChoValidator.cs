using FluentValidation;

namespace ClinicBooking.Application.Features.LichHen.Commands.GiaiPhongGiuCho;

public class GiaiPhongGiuChoValidator : AbstractValidator<GiaiPhongGiuChoCommand>
{
    public GiaiPhongGiuChoValidator()
    {
        RuleFor(x => x.IdGiuCho)
            .GreaterThan(0).WithMessage("Id giu cho phai lon hon 0.");
    }
}
