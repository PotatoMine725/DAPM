using FluentValidation;

namespace ClinicBooking.Application.Features.LichHen.Commands.TaoLichHen;

public class TaoLichHenValidator : AbstractValidator<TaoLichHenCommand>
{
    public TaoLichHenValidator()
    {
        RuleFor(x => x.IdCaLamViec)
            .GreaterThan(0).WithMessage("IdCaLamViec khong hop le.");

        RuleFor(x => x.IdDichVu)
            .GreaterThan(0).WithMessage("IdDichVu khong hop le.");

        RuleFor(x => x.IdBenhNhan!.Value)
            .GreaterThan(0)
            .When(x => x.IdBenhNhan.HasValue)
            .WithMessage("IdBenhNhan khong hop le.");

        RuleFor(x => x.IdBacSiMongMuon!.Value)
            .GreaterThan(0)
            .When(x => x.IdBacSiMongMuon.HasValue)
            .WithMessage("IdBacSiMongMuon khong hop le.");

        RuleFor(x => x.BacSiMongMuonNote)
            .MaximumLength(256).WithMessage("Ghi chu bac si mong muon toi da 256 ky tu.");

        RuleFor(x => x.TrieuChung)
            .MaximumLength(1024).WithMessage("Trieu chung toi da 1024 ky tu.");
    }
}
