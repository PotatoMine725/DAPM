using FluentValidation;

namespace ClinicBooking.Application.Features.DanhMuc.Queries.LayDichVuById;

public sealed class LayDichVuByIdValidator : AbstractValidator<LayDichVuByIdQuery>
{
    public LayDichVuByIdValidator()
    {
        RuleFor(x => x.IdDichVu)
            .GreaterThan(0).WithMessage("Id dich vu khong hop le.");
    }
}
