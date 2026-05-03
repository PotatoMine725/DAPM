using ClinicBooking.Domain.Enums;
using FluentValidation;

namespace ClinicBooking.Application.Features.NghiPhep.Commands.NopDonNghiPhep;

public sealed class NopDonNghiPhepValidator : AbstractValidator<NopDonNghiPhepCommand>
{
    public NopDonNghiPhepValidator()
    {
        RuleFor(x => x.IdBacSi).GreaterThan(0);
        RuleFor(x => x.IdCaLamViec).GreaterThan(0);
        RuleFor(x => x.LyDo).NotEmpty();
        RuleFor(x => x.LoaiNghiPhep).Must(v => Enum.IsDefined(typeof(LoaiNghiPhep), v));
    }
}
