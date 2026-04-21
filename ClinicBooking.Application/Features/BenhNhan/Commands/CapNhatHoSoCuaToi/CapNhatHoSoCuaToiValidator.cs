using FluentValidation;

namespace ClinicBooking.Application.Features.BenhNhan.Commands.CapNhatHoSoCuaToi;

public sealed class CapNhatHoSoCuaToiValidator : AbstractValidator<CapNhatHoSoCuaToiCommand>
{
    public CapNhatHoSoCuaToiValidator()
    {
        RuleFor(x => x.HoTen)
            .NotEmpty().WithMessage("Ho ten khong duoc de trong.")
            .MaximumLength(128).WithMessage("Ho ten toi da 128 ky tu.");

        RuleFor(x => x.NgaySinh!.Value)
            .LessThan(DateOnly.FromDateTime(DateTime.UtcNow))
            .When(x => x.NgaySinh.HasValue)
            .WithMessage("Ngay sinh phai nho hon ngay hien tai.");

        RuleFor(x => x.GioiTinh!.Value)
            .IsInEnum()
            .When(x => x.GioiTinh.HasValue)
            .WithMessage("Gioi tinh khong hop le.");

        RuleFor(x => x.Cccd)
            .Matches(@"^\d{12}$")
            .When(x => !string.IsNullOrWhiteSpace(x.Cccd))
            .WithMessage("CCCD phai gom 12 chu so.");

        RuleFor(x => x.DiaChi)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.DiaChi))
            .WithMessage("Dia chi toi da 500 ky tu.");
    }
}
