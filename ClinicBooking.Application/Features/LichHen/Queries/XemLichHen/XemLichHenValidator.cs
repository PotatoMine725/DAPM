using FluentValidation;

namespace ClinicBooking.Application.Features.LichHen.Queries.XemLichHen;

public class XemLichHenValidator : AbstractValidator<XemLichHenQuery>
{
    public XemLichHenValidator()
    {
        RuleFor(x => x.IdLichHen)
            .GreaterThan(0).WithMessage("Id lich hen phai lon hon 0.");
    }
}
