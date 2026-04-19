using FluentValidation;

namespace ClinicBooking.Application.Features.HangCho.Queries.XemHangChoTheoCa;

public class XemHangChoTheoCaValidator : AbstractValidator<XemHangChoTheoCaQuery>
{
    public XemHangChoTheoCaValidator()
    {
        RuleFor(x => x.IdCaLamViec)
            .GreaterThan(0).WithMessage("Id ca lam viec phai lon hon 0.");
    }
}
