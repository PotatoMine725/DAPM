using FluentValidation;

namespace ClinicBooking.Application.Features.ToaThuoc.Queries.LayToaTheoHoSoKham;

public sealed class LayToaTheoHoSoKhamValidator : AbstractValidator<LayToaTheoHoSoKhamQuery>
{
    public LayToaTheoHoSoKhamValidator()
    {
        RuleFor(x => x.IdHoSoKham)
            .GreaterThan(0).WithMessage("Id ho so kham khong hop le.");
    }
}
