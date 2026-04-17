using FluentValidation;

namespace ClinicBooking.Application.Features.LichHen.Commands.CheckInLichHen;

public class CheckInLichHenValidator : AbstractValidator<CheckInLichHenCommand>
{
    public CheckInLichHenValidator()
    {
        RuleFor(x => x.IdLichHen)
            .GreaterThan(0).WithMessage("Id lich hen phai lon hon 0.");
    }
}
