using FluentValidation;

namespace ClinicBooking.Application.Features.HangCho.Commands.GoiBenhNhanKeTiep;

public class GoiBenhNhanKeTiepValidator : AbstractValidator<GoiBenhNhanKeTiepCommand>
{
    public GoiBenhNhanKeTiepValidator()
    {
        RuleFor(x => x.IdCaLamViec)
            .GreaterThan(0).WithMessage("Id ca lam viec phai lon hon 0.");
    }
}
