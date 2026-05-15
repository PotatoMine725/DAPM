using FluentValidation;

namespace ClinicBooking.Application.Features.Doctors.Commands.CapNhatBacSi;

public sealed class CapNhatBacSiValidator : AbstractValidator<CapNhatBacSiCommand>
{
    public CapNhatBacSiValidator()
    {
        RuleFor(x => x.IdBacSi).GreaterThan(0);
        RuleFor(x => x.HoTen).NotEmpty().MaximumLength(100);
        RuleFor(x => x.IdChuyenKhoa).GreaterThan(0);
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.SoDienThoai).Matches("^0[0-9]{9}$").When(x => !string.IsNullOrWhiteSpace(x.SoDienThoai));
        RuleFor(x => x.NamKinhNghiem).GreaterThanOrEqualTo(0).When(x => x.NamKinhNghiem.HasValue);
    }
}
