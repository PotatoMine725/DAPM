using FluentValidation;

namespace ClinicBooking.Application.Features.BacSi.Queries.LayLichLamViecCuaToi;

public sealed class LayLichLamViecCuaToiValidator : AbstractValidator<LayLichLamViecCuaToiQuery>
{
    public LayLichLamViecCuaToiValidator()
    {
        RuleFor(x => x.TuNgay)
            .LessThanOrEqualTo(x => x.DenNgay)
            .WithMessage("TuNgay phai nho hon hoac bang DenNgay.");
    }
}
