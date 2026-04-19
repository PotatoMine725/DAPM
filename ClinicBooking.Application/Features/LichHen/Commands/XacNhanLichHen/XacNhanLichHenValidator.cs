using FluentValidation;

namespace ClinicBooking.Application.Features.LichHen.Commands.XacNhanLichHen;

public class XacNhanLichHenValidator : AbstractValidator<XacNhanLichHenCommand>
{
    public XacNhanLichHenValidator()
    {
        RuleFor(x => x.IdLichHen)
            .GreaterThan(0).WithMessage("Id lich hen phai lon hon 0.");
    }
}
