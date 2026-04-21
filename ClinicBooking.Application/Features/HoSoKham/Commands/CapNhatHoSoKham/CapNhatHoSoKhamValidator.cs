using FluentValidation;

namespace ClinicBooking.Application.Features.HoSoKham.Commands.CapNhatHoSoKham;

public sealed class CapNhatHoSoKhamValidator : AbstractValidator<CapNhatHoSoKhamCommand>
{
    public CapNhatHoSoKhamValidator()
    {
        RuleFor(x => x.IdHoSoKham)
            .GreaterThan(0).WithMessage("Id ho so kham khong hop le.");

        RuleFor(x => x.ChanDoan)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrWhiteSpace(x.ChanDoan))
            .WithMessage("Chan doan toi da 2000 ky tu.");

        RuleFor(x => x.KetQuaKham)
            .MaximumLength(4000)
            .When(x => !string.IsNullOrWhiteSpace(x.KetQuaKham))
            .WithMessage("Ket qua kham toi da 4000 ky tu.");

        RuleFor(x => x.GhiChu)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrWhiteSpace(x.GhiChu))
            .WithMessage("Ghi chu toi da 2000 ky tu.");
    }
}
