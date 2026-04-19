using FluentValidation;

namespace ClinicBooking.Application.Features.HangCho.Commands.HoanThanhLuotKham;

public class HoanThanhLuotKhamValidator : AbstractValidator<HoanThanhLuotKhamCommand>
{
    public HoanThanhLuotKhamValidator()
    {
        RuleFor(x => x.IdHangCho)
            .GreaterThan(0).WithMessage("Id hang cho phai lon hon 0.");
    }
}
