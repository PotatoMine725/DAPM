using FluentValidation;

namespace ClinicBooking.Application.Features.ToaThuoc.Queries.LichSuToaThuocTheoBenhNhan;

public class LichSuToaThuocTheoBenhNhanValidator : AbstractValidator<LichSuToaThuocTheoBenhNhanQuery>
{
    public LichSuToaThuocTheoBenhNhanValidator()
    {
        RuleFor(x => x.IdBenhNhan)
            .GreaterThan(0)
            .WithMessage("Id benh nhan phai lon hon 0.");

        RuleFor(x => x.SoTrang)
            .GreaterThan(0)
            .WithMessage("So trang phai lon hon 0.");

        RuleFor(x => x.KichThuocTrang)
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .WithMessage("Kich thuoc trang phai tu 1 den 100.");
    }
}
