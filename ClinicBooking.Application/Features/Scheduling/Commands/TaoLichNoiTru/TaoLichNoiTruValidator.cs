using FluentValidation;

namespace ClinicBooking.Application.Features.Scheduling.Commands.TaoLichNoiTru;

public sealed class TaoLichNoiTruValidator : AbstractValidator<TaoLichNoiTruCommand>
{
    public TaoLichNoiTruValidator()
    {
        RuleFor(x => x.IdBacSi).GreaterThan(0);
        RuleFor(x => x.IdPhong).GreaterThan(0);
        RuleFor(x => x.IdDinhNghiaCa).GreaterThan(0);
        RuleFor(x => x.NgayTrongTuan).InclusiveBetween(0, 6);
    }
}
