using FluentValidation;

namespace ClinicBooking.Application.Features.LichHen.Commands.TaoGiuCho;

public class TaoGiuChoValidator : AbstractValidator<TaoGiuChoCommand>
{
    public TaoGiuChoValidator()
    {
        RuleFor(x => x.IdCaLamViec)
            .GreaterThan(0).WithMessage("IdCaLamViec khong hop le.");

        RuleFor(x => x.IdBenhNhan)
            .GreaterThan(0).WithMessage("IdBenhNhan khong hop le.");
    }
}
