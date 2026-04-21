using FluentValidation;

namespace ClinicBooking.Application.Features.DanhMuc.Commands.XoaDichVu;

public sealed class XoaDichVuValidator : AbstractValidator<XoaDichVuCommand>
{
    public XoaDichVuValidator()
    {
        RuleFor(x => x.IdDichVu)
            .GreaterThan(0).WithMessage("Id dich vu khong hop le.");
    }
}
