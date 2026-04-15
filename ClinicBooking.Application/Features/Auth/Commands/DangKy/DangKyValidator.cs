using FluentValidation;

namespace ClinicBooking.Application.Features.Auth.Commands.DangKy;

public class DangKyValidator : AbstractValidator<DangKyCommand>
{
    public DangKyValidator()
    {
        RuleFor(x => x.TenDangNhap)
            .NotEmpty().WithMessage("Ten dang nhap khong duoc de trong.")
            .MinimumLength(4).WithMessage("Ten dang nhap toi thieu 4 ky tu.")
            .MaximumLength(50).WithMessage("Ten dang nhap toi da 50 ky tu.")
            .Matches("^[a-zA-Z0-9_.]+$")
            .WithMessage("Ten dang nhap chi duoc chua chu, so, dau chan va dau cham.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email khong duoc de trong.")
            .EmailAddress().WithMessage("Email khong hop le.")
            .MaximumLength(128).WithMessage("Email toi da 128 ky tu.");

        RuleFor(x => x.SoDienThoai)
            .NotEmpty().WithMessage("So dien thoai khong duoc de trong.")
            .Matches(@"^0\d{9}$")
            .WithMessage("So dien thoai phai gom 10 chu so va bat dau bang so 0.");

        RuleFor(x => x.MatKhau)
            .NotEmpty().WithMessage("Mat khau khong duoc de trong.")
            .MinimumLength(8).WithMessage("Mat khau toi thieu 8 ky tu.")
            .MaximumLength(64).WithMessage("Mat khau toi da 64 ky tu.")
            .Matches("[A-Z]").WithMessage("Mat khau phai co it nhat 1 chu hoa.")
            .Matches("[a-z]").WithMessage("Mat khau phai co it nhat 1 chu thuong.")
            .Matches("[0-9]").WithMessage("Mat khau phai co it nhat 1 chu so.");

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
    }
}
