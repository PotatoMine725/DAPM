using FluentValidation;

namespace ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachDichVu;

public sealed class DanhSachDichVuValidator : AbstractValidator<DanhSachDichVuQuery>
{
    public DanhSachDichVuValidator()
    {
        RuleFor(x => x.SoTrang)
            .GreaterThan(0).WithMessage("So trang phai lon hon 0.");

        RuleFor(x => x.KichThuocTrang)
            .InclusiveBetween(1, 100).WithMessage("Kich thuoc trang phai trong khoang 1-100.");

        RuleFor(x => x.IdChuyenKhoa)
            .GreaterThan(0)
            .When(x => x.IdChuyenKhoa.HasValue)
            .WithMessage("Id chuyen khoa khong hop le.");

        RuleFor(x => x.TuKhoa)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.TuKhoa))
            .WithMessage("Tu khoa toi da 100 ky tu.");
    }
}
