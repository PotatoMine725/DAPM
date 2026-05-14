using FluentValidation;

namespace ClinicBooking.Application.Features.Auth.Commands.KichHoatTaiKhoanWalkIn;

public class KichHoatTaiKhoanWalkInValidator : AbstractValidator<KichHoatTaiKhoanWalkInCommand>
{
    public KichHoatTaiKhoanWalkInValidator()
    {
        RuleFor(x => x.IdTaiKhoan).GreaterThan(0);
        RuleFor(x => x.MaOtp).NotEmpty().Length(6);
        RuleFor(x => x.TenDangNhap).NotEmpty().MinimumLength(3).MaximumLength(50);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.MatKhau).NotEmpty().MinimumLength(6);
        RuleFor(x => x)
            .Must(x =>
                !string.IsNullOrWhiteSpace(x.Cccd) ||
                (x.NgaySinh.HasValue && !string.IsNullOrWhiteSpace(x.HoTen)))
            .WithMessage("Can nhap CCCD hoac (Ngay sinh + Ho ten) de xac minh danh tinh.");
    }
}
