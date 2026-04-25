using FluentValidation;

namespace ClinicBooking.Application.Features.Scheduling.Commands.DuyetCaLamViec;

public sealed class DuyetCaLamViecValidator : AbstractValidator<DuyetCaLamViecCommand>
{
    public DuyetCaLamViecValidator()
    {
        RuleFor(x => x.IdCaLamViec).GreaterThan(0);
        RuleFor(x => x.IdAdminDuyet).GreaterThan(0);
        RuleFor(x => x.LyDoTuChoi)
            .NotEmpty()
            .When(x => !x.ChapNhan)
            .WithMessage("Ly do tu choi khong duoc de trong khi tu choi.");
    }
}
