using FluentValidation;

namespace ClinicBooking.Application.Features.DanhMuc.Commands.TaoChuyenKhoa;

public sealed class TaoChuyenKhoaValidator : AbstractValidator<TaoChuyenKhoaCommand>
{
    public TaoChuyenKhoaValidator()
    {
        RuleFor(x => x.TenChuyenKhoa)
            .NotEmpty().WithMessage("Ten chuyen khoa khong duoc de trong.")
            .MaximumLength(450).WithMessage("Ten chuyen khoa toi da 450 ky tu.");

        RuleFor(x => x.MoTa)
            .MaximumLength(4000)
            .When(x => !string.IsNullOrEmpty(x.MoTa))
            .WithMessage("Mo ta toi da 4000 ky tu.");

        RuleFor(x => x.ThoiGianSlotMacDinh)
            .GreaterThan(0).WithMessage("Thoi gian slot mac dinh phai lon hon 0.")
            .LessThanOrEqualTo(1440).WithMessage("Thoi gian slot mac dinh khong hop le.");

        RuleFor(x => x)
            .Must(x => !x.GioMoDatLich.HasValue || !x.GioDongDatLich.HasValue
                || x.GioMoDatLich.Value < x.GioDongDatLich.Value)
            .WithMessage("Gio mo dat lich phai truoc gio dong dat lich.");
    }
}
