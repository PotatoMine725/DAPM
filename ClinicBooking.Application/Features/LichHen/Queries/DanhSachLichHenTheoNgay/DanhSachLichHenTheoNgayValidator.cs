using FluentValidation;

namespace ClinicBooking.Application.Features.LichHen.Queries.DanhSachLichHenTheoNgay;

public class DanhSachLichHenTheoNgayValidator : AbstractValidator<DanhSachLichHenTheoNgayQuery>
{
    public DanhSachLichHenTheoNgayValidator()
    {
        RuleFor(x => x.Ngay)
            .NotEqual(default(DateOnly))
            .WithMessage("Ngay khong hop le.");
    }
}
