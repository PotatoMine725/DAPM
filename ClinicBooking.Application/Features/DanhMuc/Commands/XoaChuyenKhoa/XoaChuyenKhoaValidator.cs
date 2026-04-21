using FluentValidation;

namespace ClinicBooking.Application.Features.DanhMuc.Commands.XoaChuyenKhoa;

public sealed class XoaChuyenKhoaValidator : AbstractValidator<XoaChuyenKhoaCommand>
{
    public XoaChuyenKhoaValidator()
    {
        RuleFor(x => x.IdChuyenKhoa)
            .GreaterThan(0).WithMessage("Id chuyen khoa khong hop le.");
    }
}
