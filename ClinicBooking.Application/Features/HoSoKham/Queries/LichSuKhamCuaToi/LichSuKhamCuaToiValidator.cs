using FluentValidation;

namespace ClinicBooking.Application.Features.HoSoKham.Queries.LichSuKhamCuaToi;

public sealed class LichSuKhamCuaToiValidator : AbstractValidator<LichSuKhamCuaToiQuery>
{
    public LichSuKhamCuaToiValidator()
    {
        RuleFor(x => x.SoTrang)
            .GreaterThan(0).WithMessage("So trang phai lon hon 0.");

        RuleFor(x => x.KichThuocTrang)
            .InclusiveBetween(1, 200)
            .WithMessage("Kich thuoc trang phai trong khoang 1-200.");
    }
}
