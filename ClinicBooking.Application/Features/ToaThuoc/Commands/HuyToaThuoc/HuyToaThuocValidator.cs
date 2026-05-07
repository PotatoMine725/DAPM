using FluentValidation;

namespace ClinicBooking.Application.Features.ToaThuoc.Commands.HuyToaThuoc;

public class HuyToaThuocValidator : AbstractValidator<HuyToaThuocCommand>
{
    public HuyToaThuocValidator()
    {
        RuleFor(x => x.IdHoSoKham)
            .GreaterThan(0)
            .WithMessage("Id ho so kham phai lon hon 0.");

        RuleFor(x => x.IdToaThuoc)
            .GreaterThan(0)
            .WithMessage("Id toa thuoc phai lon hon 0.");
    }
}
