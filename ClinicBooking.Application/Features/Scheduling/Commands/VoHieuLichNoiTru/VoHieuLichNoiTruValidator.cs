using FluentValidation;

namespace ClinicBooking.Application.Features.Scheduling.Commands.VoHieuLichNoiTru;

public sealed class VoHieuLichNoiTruValidator : AbstractValidator<VoHieuLichNoiTruCommand>
{
    public VoHieuLichNoiTruValidator()
    {
        RuleFor(x => x.IdLichNoiTru).GreaterThan(0);
    }
}
