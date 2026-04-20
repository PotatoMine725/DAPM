using FluentValidation;

namespace ClinicBooking.Application.Features.DanhMuc.Commands.CapNhatDichVu;

public sealed class CapNhatDichVuValidator : AbstractValidator<CapNhatDichVuCommand>
{
    public CapNhatDichVuValidator()
    {
        RuleFor(x => x.IdDichVu)
            .GreaterThan(0).WithMessage("Id dich vu khong hop le.");

        RuleFor(x => x.IdChuyenKhoa)
            .GreaterThan(0).WithMessage("Id chuyen khoa khong hop le.");

        RuleFor(x => x.TenDichVu)
            .NotEmpty().WithMessage("Ten dich vu khong duoc de trong.")
            .MaximumLength(300).WithMessage("Ten dich vu toi da 300 ky tu.");

        RuleFor(x => x.MoTa)
            .MaximumLength(4000)
            .When(x => !string.IsNullOrWhiteSpace(x.MoTa))
            .WithMessage("Mo ta toi da 4000 ky tu.");

        RuleFor(x => x.ThoiGianUocTinh)
            .InclusiveBetween(1, 1440)
            .When(x => x.ThoiGianUocTinh.HasValue)
            .WithMessage("Thoi gian uoc tinh phai trong khoang 1-1440 phut.");
    }
}
