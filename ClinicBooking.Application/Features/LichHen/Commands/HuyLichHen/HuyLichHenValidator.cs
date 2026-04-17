using FluentValidation;

namespace ClinicBooking.Application.Features.LichHen.Commands.HuyLichHen;

public class HuyLichHenValidator : AbstractValidator<HuyLichHenCommand>
{
    public HuyLichHenValidator()
    {
        RuleFor(x => x.IdLichHen)
            .GreaterThan(0).WithMessage("Id lich hen phai lon hon 0.");

        RuleFor(x => x.LyDo)
            .NotEmpty().WithMessage("Ly do huy khong duoc de trong.")
            .MaximumLength(500).WithMessage("Ly do huy toi da 500 ky tu.");
    }
}
