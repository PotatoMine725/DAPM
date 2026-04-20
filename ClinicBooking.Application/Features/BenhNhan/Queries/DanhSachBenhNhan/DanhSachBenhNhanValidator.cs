using FluentValidation;

namespace ClinicBooking.Application.Features.BenhNhan.Queries.DanhSachBenhNhan;

public sealed class DanhSachBenhNhanValidator : AbstractValidator<DanhSachBenhNhanQuery>
{
    public DanhSachBenhNhanValidator()
    {
        RuleFor(x => x.SoTrang)
            .GreaterThan(0).WithMessage("So trang phai lon hon 0.");

        RuleFor(x => x.KichThuocTrang)
            .InclusiveBetween(1, 200)
            .WithMessage("Kich thuoc trang phai trong khoang 1-200.");

        RuleFor(x => x.TuKhoa)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.TuKhoa))
            .WithMessage("Tu khoa toi da 200 ky tu.");
    }
}
