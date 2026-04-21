using FluentValidation;

namespace ClinicBooking.Application.Features.Thuoc.Commands.TaoThuoc;

public sealed class TaoThuocValidator : AbstractValidator<TaoThuocCommand>
{
    public TaoThuocValidator()
    {
        RuleFor(x => x.TenThuoc)
            .NotEmpty().WithMessage("Ten thuoc khong duoc de trong.")
            .MaximumLength(300).WithMessage("Ten thuoc toi da 300 ky tu.");

        RuleFor(x => x.HoatChat)
            .MaximumLength(300)
            .When(x => !string.IsNullOrWhiteSpace(x.HoatChat))
            .WithMessage("Hoat chat toi da 300 ky tu.");

        RuleFor(x => x.DonVi)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.DonVi))
            .WithMessage("Don vi toi da 100 ky tu.");

        RuleFor(x => x.GhiChu)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrWhiteSpace(x.GhiChu))
            .WithMessage("Ghi chu toi da 1000 ky tu.");
    }
}
