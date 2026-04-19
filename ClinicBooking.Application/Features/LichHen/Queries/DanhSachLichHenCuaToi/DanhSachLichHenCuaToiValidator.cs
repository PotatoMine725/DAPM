using ClinicBooking.Application.Common.Constants;
using FluentValidation;

namespace ClinicBooking.Application.Features.LichHen.Queries.DanhSachLichHenCuaToi;

public class DanhSachLichHenCuaToiValidator : AbstractValidator<DanhSachLichHenCuaToiQuery>
{
    public DanhSachLichHenCuaToiValidator()
    {
        RuleFor(x => x.SoTrang)
            .GreaterThan(0).WithMessage("So trang phai lon hon 0.");

        RuleFor(x => x.KichThuocTrang)
            .InclusiveBetween(1, LichHenConstants.KichThuocTrangToiDa)
            .WithMessage($"Kich thuoc trang phai tu 1 den {LichHenConstants.KichThuocTrangToiDa}.");

        RuleFor(x => x.TrangThai!.Value)
            .IsInEnum().When(x => x.TrangThai.HasValue)
            .WithMessage("Trang thai khong hop le.");
    }
}
