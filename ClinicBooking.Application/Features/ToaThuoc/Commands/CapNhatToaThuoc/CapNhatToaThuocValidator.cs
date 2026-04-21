using FluentValidation;

namespace ClinicBooking.Application.Features.ToaThuoc.Commands.CapNhatToaThuoc;

public sealed class CapNhatToaThuocValidator : AbstractValidator<CapNhatToaThuocCommand>
{
    public CapNhatToaThuocValidator()
    {
        RuleFor(x => x.IdHoSoKham)
            .GreaterThan(0).WithMessage("Id ho so kham khong hop le.");

        RuleFor(x => x.DanhSachThuoc)
            .NotNull().WithMessage("Danh sach thuoc khong duoc rong.")
            .Must(x => x.Count > 0).WithMessage("Danh sach thuoc khong duoc rong.");

        RuleForEach(x => x.DanhSachThuoc).ChildRules(item =>
        {
            item.RuleFor(x => x.IdThuoc)
                .GreaterThan(0).WithMessage("Id thuoc khong hop le.");

            item.RuleFor(x => x.LieuLuong)
                .MaximumLength(255)
                .When(x => !string.IsNullOrWhiteSpace(x.LieuLuong))
                .WithMessage("Lieu luong toi da 255 ky tu.");

            item.RuleFor(x => x.CachDung)
                .MaximumLength(1000)
                .When(x => !string.IsNullOrWhiteSpace(x.CachDung))
                .WithMessage("Cach dung toi da 1000 ky tu.");

            item.RuleFor(x => x.SoNgayDung)
                .InclusiveBetween(1, 365)
                .When(x => x.SoNgayDung.HasValue)
                .WithMessage("So ngay dung phai trong khoang 1-365.");

            item.RuleFor(x => x.GhiChu)
                .MaximumLength(1000)
                .When(x => !string.IsNullOrWhiteSpace(x.GhiChu))
                .WithMessage("Ghi chu toi da 1000 ky tu.");
        });
    }
}
