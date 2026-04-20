using FluentValidation;

namespace ClinicBooking.Application.Features.DanhMuc.Queries.LayChuyenKhoaById;

public sealed class LayChuyenKhoaByIdValidator : AbstractValidator<LayChuyenKhoaByIdQuery>
{
    public LayChuyenKhoaByIdValidator()
    {
        RuleFor(x => x.IdChuyenKhoa)
            .GreaterThan(0).WithMessage("Id chuyen khoa khong hop le.");
    }
}
