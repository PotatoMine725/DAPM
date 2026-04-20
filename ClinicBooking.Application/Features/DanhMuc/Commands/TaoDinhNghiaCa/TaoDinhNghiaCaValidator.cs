using FluentValidation;

namespace ClinicBooking.Application.Features.DanhMuc.Commands.TaoDinhNghiaCa;

public sealed class TaoDinhNghiaCaValidator : AbstractValidator<TaoDinhNghiaCaCommand>
{
    public TaoDinhNghiaCaValidator()
    {
        RuleFor(x => x.TenCa)
            .NotEmpty().WithMessage("Ten ca khong duoc de trong.")
            .MaximumLength(450).WithMessage("Ten ca toi da 450 ky tu.");

        RuleFor(x => x.MoTa)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrWhiteSpace(x.MoTa))
            .WithMessage("Mo ta toi da 2000 ky tu.");

        RuleFor(x => x)
            .Must(x => x.GioBatDauMacDinh < x.GioKetThucMacDinh)
            .WithMessage("Gio bat dau mac dinh phai truoc gio ket thuc mac dinh.");
    }
}
