using FluentValidation;

namespace ClinicBooking.Application.Features.Auth.Commands.DangNhap;

public class DangNhapValidator : AbstractValidator<DangNhapCommand>
{
    public DangNhapValidator()
    {
        RuleFor(x => x.TenDangNhapHoacEmail)
            .NotEmpty().WithMessage("Vui long nhap ten dang nhap hoac email.")
            .MaximumLength(128).WithMessage("Ten dang nhap hoac email toi da 128 ky tu.");

        RuleFor(x => x.MatKhau)
            .NotEmpty().WithMessage("Vui long nhap mat khau.")
            .MaximumLength(64).WithMessage("Mat khau toi da 64 ky tu.");
    }
}
