using FluentValidation;

namespace ClinicBooking.Application.Features.Auth.Commands.LamMoiToken;

public class LamMoiTokenValidator : AbstractValidator<LamMoiTokenCommand>
{
    public LamMoiTokenValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token khong duoc de trong.")
            .MaximumLength(256).WithMessage("Refresh token khong hop le.");
    }
}
