using FluentValidation;

namespace ClinicBooking.Application.Features.Auth.Commands.DangXuat;

public class DangXuatValidator : AbstractValidator<DangXuatCommand>
{
    public DangXuatValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token khong duoc de trong.")
            .MaximumLength(256).WithMessage("Refresh token khong hop le.");
    }
}
