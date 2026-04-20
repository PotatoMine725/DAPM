using FluentValidation;

namespace ClinicBooking.Application.Features.DanhMuc.Commands.TaoPhong;

public sealed class TaoPhongValidator : AbstractValidator<TaoPhongCommand>
{
    public TaoPhongValidator()
    {
        RuleFor(x => x.MaPhong)
            .NotEmpty().WithMessage("Ma phong khong duoc de trong.")
            .MaximumLength(50).WithMessage("Ma phong toi da 50 ky tu.");

        RuleFor(x => x.TenPhong)
            .NotEmpty().WithMessage("Ten phong khong duoc de trong.")
            .MaximumLength(200).WithMessage("Ten phong toi da 200 ky tu.");

        RuleFor(x => x.SucChua)
            .InclusiveBetween(1, 1000)
            .When(x => x.SucChua.HasValue)
            .WithMessage("Suc chua phai trong khoang 1-1000.");

        RuleFor(x => x.TrangBi)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrWhiteSpace(x.TrangBi))
            .WithMessage("Trang bi toi da 2000 ky tu.");
    }
}
