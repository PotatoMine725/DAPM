using ClinicBooking.Domain.Enums;
using FluentValidation;

namespace ClinicBooking.Application.Features.BacSi.Commands.CapNhatBacSi;

public sealed class CapNhatBacSiValidator : AbstractValidator<CapNhatBacSiCommand>
{
    public CapNhatBacSiValidator()
    {
        RuleFor(x => x.HoTen)
            .NotEmpty().WithMessage("Ho ten khong duoc de trong.");

        RuleFor(x => x.IdChuyenKhoa)
            .GreaterThan(0).WithMessage("Id chuyen khoa phai lon hon 0.");

        RuleFor(x => x.LoaiHopDong)
            .Must(value => Enum.IsDefined(typeof(LoaiHopDong), value))
            .WithMessage("Loai hop dong khong hop le.");

        RuleFor(x => x.TrangThai)
            .Must(value => Enum.IsDefined(typeof(TrangThaiBacSi), value))
            .WithMessage("Trang thai khong hop le.");
    }
}
