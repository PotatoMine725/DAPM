using FluentValidation;

namespace ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachChuyenKhoa;

public sealed class DanhSachChuyenKhoaValidator : AbstractValidator<DanhSachChuyenKhoaQuery>
{
    public DanhSachChuyenKhoaValidator()
    {
        RuleFor(x => x.SoTrang)
            .GreaterThan(0).WithMessage("So trang phai lon hon 0.");

        RuleFor(x => x.KichThuocTrang)
            .InclusiveBetween(1, 100).WithMessage("Kich thuoc trang phai trong khoang 1-100.");

        RuleFor(x => x.TuKhoa)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.TuKhoa))
            .WithMessage("Tu khoa toi da 100 ky tu.");
    }
}
