using FluentValidation;

namespace ClinicBooking.Application.Features.Scheduling.Commands.CapNhatLichNoiTru;

public sealed class CapNhatLichNoiTruValidator : AbstractValidator<CapNhatLichNoiTruCommand>
{
    public CapNhatLichNoiTruValidator()
    {
        RuleFor(x => x.IdLichNoiTru).GreaterThan(0);
        RuleFor(x => x.IdPhong).GreaterThan(0);
        RuleFor(x => x.IdDinhNghiaCa).GreaterThan(0);
        RuleFor(x => x.NgayTrongTuan).InclusiveBetween(0, 6);
    }
}
