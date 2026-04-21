using FluentValidation;

namespace ClinicBooking.Application.Features.DanhMuc.Queries.LayPhongById;

public sealed class LayPhongByIdValidator : AbstractValidator<LayPhongByIdQuery>
{
    public LayPhongByIdValidator()
    {
        RuleFor(x => x.IdPhong)
            .GreaterThan(0).WithMessage("Id phong khong hop le.");
    }
}
