using FluentValidation;

namespace ClinicBooking.Application.Features.HoSoKham.Queries.LichSuHoSoKhamCuaBacSi;

public class LichSuHoSoKhamCuaBacSiValidator : AbstractValidator<LichSuHoSoKhamCuaBacSiQuery>
{
    public LichSuHoSoKhamCuaBacSiValidator()
    {
        RuleFor(x => x.IdBacSi)
            .GreaterThan(0)
            .WithMessage("Id bac si phai lon hon 0.");

        RuleFor(x => x.SoTrang)
            .GreaterThan(0)
            .WithMessage("So trang phai lon hon 0.");

        RuleFor(x => x.KichThuocTrang)
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .WithMessage("Kich thuoc trang phai tu 1 den 100.");
    }
}
