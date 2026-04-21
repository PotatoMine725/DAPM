using FluentValidation;

namespace ClinicBooking.Application.Features.DanhMuc.Commands.XoaPhong;

public sealed class XoaPhongValidator : AbstractValidator<XoaPhongCommand>
{
    public XoaPhongValidator()
    {
        RuleFor(x => x.IdPhong)
            .GreaterThan(0).WithMessage("Id phong khong hop le.");
    }
}
