using FluentValidation;

namespace ClinicBooking.Application.Features.NghiPhep.Commands.TuChoiDonNghiPhep;

public sealed class TuChoiDonNghiPhepValidator : AbstractValidator<TuChoiDonNghiPhepCommand>
{
    public TuChoiDonNghiPhepValidator()
    {
        RuleFor(x => x.IdDonNghiPhep).GreaterThan(0);
        RuleFor(x => x.IdNguoiDuyet).GreaterThan(0);
        RuleFor(x => x.LyDoTuChoi).NotEmpty();
    }
}
