using FluentValidation;

namespace ClinicBooking.Application.Features.NghiPhep.Commands.DuyetDonNghiPhep;

public sealed class DuyetDonNghiPhepValidator : AbstractValidator<DuyetDonNghiPhepCommand>
{
    public DuyetDonNghiPhepValidator()
    {
        RuleFor(x => x.IdDonNghiPhep).GreaterThan(0);
        RuleFor(x => x.IdNguoiDuyet).GreaterThan(0);
    }
}
