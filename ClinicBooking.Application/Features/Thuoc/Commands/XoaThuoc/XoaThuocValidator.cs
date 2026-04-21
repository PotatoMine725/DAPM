using FluentValidation;

namespace ClinicBooking.Application.Features.Thuoc.Commands.XoaThuoc;

public sealed class XoaThuocValidator : AbstractValidator<XoaThuocCommand>
{
    public XoaThuocValidator()
    {
        RuleFor(x => x.IdThuoc)
            .GreaterThan(0).WithMessage("Id thuoc khong hop le.");
    }
}
