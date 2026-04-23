using FluentValidation;

namespace ClinicBooking.Application.Features.HangCho.Queries.ThuTuCuaToi;

public sealed class ThuTuCuaToiValidator : AbstractValidator<ThuTuCuaToiQuery>
{
    public ThuTuCuaToiValidator()
    {
        RuleFor(x => x.IdCaLamViec)
            .GreaterThan(0)
            .WithMessage("Id ca lam viec phai lon hon 0.");
    }
}
