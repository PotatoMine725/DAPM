using FluentValidation;

namespace ClinicBooking.Application.Features.LichHen.Commands.DoiLichHen;

public class DoiLichHenValidator : AbstractValidator<DoiLichHenCommand>
{
    public DoiLichHenValidator()
    {
        RuleFor(x => x.IdLichHenCu)
            .GreaterThan(0).WithMessage("IdLichHenCu khong hop le.");

        RuleFor(x => x.IdCaLamViecMoi)
            .GreaterThan(0).WithMessage("IdCaLamViecMoi khong hop le.");

        RuleFor(x => x.IdDichVuMoi!.Value)
            .GreaterThan(0)
            .When(x => x.IdDichVuMoi.HasValue)
            .WithMessage("IdDichVuMoi khong hop le.");

        RuleFor(x => x.IdBacSiMongMuon!.Value)
            .GreaterThan(0)
            .When(x => x.IdBacSiMongMuon.HasValue)
            .WithMessage("IdBacSiMongMuon khong hop le.");

        RuleFor(x => x.BacSiMongMuonNote)
            .MaximumLength(256).WithMessage("Ghi chu bac si mong muon toi da 256 ky tu.");

        RuleFor(x => x.TrieuChung)
            .MaximumLength(1024).WithMessage("Trieu chung toi da 1024 ky tu.");

        RuleFor(x => x.LyDo)
            .MaximumLength(512).WithMessage("Ly do doi lich toi da 512 ky tu.");
    }
}
