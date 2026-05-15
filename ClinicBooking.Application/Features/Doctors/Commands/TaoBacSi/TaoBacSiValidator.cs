using FluentValidation;

namespace ClinicBooking.Application.Features.Doctors.Commands.TaoBacSi;

public sealed class TaoBacSiValidator : AbstractValidator<TaoBacSiCommand>
{
    public TaoBacSiValidator()
    {
        RuleFor(x => x.TenDangNhap).NotEmpty().MinimumLength(3).MaximumLength(50);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(100);
        RuleFor(x => x.SoDienThoai).NotEmpty().Matches("^0[0-9]{9}$").WithMessage("So dien thoai khong hop le.");
        RuleFor(x => x.MatKhau).NotEmpty().MinimumLength(6).MaximumLength(100);
        RuleFor(x => x.HoTen).NotEmpty().MaximumLength(100);
        RuleFor(x => x.IdChuyenKhoa).GreaterThan(0);
        RuleFor(x => x.NamKinhNghiem).GreaterThanOrEqualTo(0).When(x => x.NamKinhNghiem.HasValue);
    }
}
