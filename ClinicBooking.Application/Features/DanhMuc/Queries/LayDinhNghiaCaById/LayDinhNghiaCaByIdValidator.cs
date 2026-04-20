using FluentValidation;

namespace ClinicBooking.Application.Features.DanhMuc.Queries.LayDinhNghiaCaById;

public sealed class LayDinhNghiaCaByIdValidator : AbstractValidator<LayDinhNghiaCaByIdQuery>
{
    public LayDinhNghiaCaByIdValidator()
    {
        RuleFor(x => x.IdDinhNghiaCa)
            .GreaterThan(0).WithMessage("Id dinh nghia ca khong hop le.");
    }
}
