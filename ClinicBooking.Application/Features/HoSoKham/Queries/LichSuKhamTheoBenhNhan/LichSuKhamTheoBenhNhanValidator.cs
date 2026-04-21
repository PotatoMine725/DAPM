using FluentValidation;

namespace ClinicBooking.Application.Features.HoSoKham.Queries.LichSuKhamTheoBenhNhan;

public sealed class LichSuKhamTheoBenhNhanValidator : AbstractValidator<LichSuKhamTheoBenhNhanQuery>
{
    public LichSuKhamTheoBenhNhanValidator()
    {
        RuleFor(x => x.IdBenhNhan)
            .GreaterThan(0).WithMessage("Id benh nhan khong hop le.");

        RuleFor(x => x.SoTrang)
            .GreaterThan(0).WithMessage("So trang phai lon hon 0.");

        RuleFor(x => x.KichThuocTrang)
            .InclusiveBetween(1, 200)
            .WithMessage("Kich thuoc trang phai trong khoang 1-200.");
    }
}
