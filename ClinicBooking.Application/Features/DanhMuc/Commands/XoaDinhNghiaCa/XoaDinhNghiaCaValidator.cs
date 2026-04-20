using FluentValidation;

namespace ClinicBooking.Application.Features.DanhMuc.Commands.XoaDinhNghiaCa;

public sealed class XoaDinhNghiaCaValidator : AbstractValidator<XoaDinhNghiaCaCommand>
{
    public XoaDinhNghiaCaValidator()
    {
        RuleFor(x => x.IdDinhNghiaCa)
            .GreaterThan(0).WithMessage("Id dinh nghia ca khong hop le.");
    }
}
