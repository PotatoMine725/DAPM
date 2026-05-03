using FluentValidation;

namespace ClinicBooking.Application.Features.Scheduling.Commands.DangKyCaLamViec;

public sealed class DangKyCaLamViecValidator : AbstractValidator<DangKyCaLamViecCommand>
{
    public DangKyCaLamViecValidator()
    {
        RuleFor(x => x.IdCaLamViec)
            .GreaterThan(0).WithMessage("Id ca lam viec phai lon hon 0.");
    }
}
