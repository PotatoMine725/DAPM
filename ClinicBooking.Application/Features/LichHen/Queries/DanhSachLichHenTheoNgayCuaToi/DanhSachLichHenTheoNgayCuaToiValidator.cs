using FluentValidation;

namespace ClinicBooking.Application.Features.LichHen.Queries.DanhSachLichHenTheoNgayCuaToi;

public sealed class DanhSachLichHenTheoNgayCuaToiValidator : AbstractValidator<DanhSachLichHenTheoNgayCuaToiQuery>
{
    public DanhSachLichHenTheoNgayCuaToiValidator()
    {
        RuleFor(x => x.Ngay)
            .NotEmpty()
            .WithMessage("Ngay khong duoc de trong.");
    }
}
