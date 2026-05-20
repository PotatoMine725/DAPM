using ClinicBooking.Application.Abstractions.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.Doctors.Commands.TaoBacSi;

public sealed class TaoBacSiValidator : AbstractValidator<TaoBacSiCommand>
{
    public TaoBacSiValidator(IAppDbContext db)
    {
        RuleFor(x => x.TenDangNhap)
            .NotEmpty().WithMessage("Tên đăng nhập không được để trống.")
            .MinimumLength(3).WithMessage("Tên đăng nhập phải có ít nhất 3 ký tự.")
            .MustAsync(async (ten, ct) => !await db.TaiKhoan.AnyAsync(t => t.TenDangNhap == ten, ct))
            .WithMessage("Tên đăng nhập đã tồn tại.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email không được để trống.")
            .EmailAddress().WithMessage("Email không hợp lệ.")
            .MustAsync(async (email, ct) => !await db.TaiKhoan.AnyAsync(t => t.Email == email, ct))
            .WithMessage("Email đã được sử dụng.");

        RuleFor(x => x.SoDienThoai)
            .NotEmpty().WithMessage("Số điện thoại không được để trống.")
            .MustAsync(async (sdt, ct) => !await db.TaiKhoan.AnyAsync(t => t.SoDienThoai == sdt, ct))
            .WithMessage("Số điện thoại đã được sử dụng.");

        RuleFor(x => x.HoTen)
            .NotEmpty().WithMessage("Họ tên không được để trống.");

        RuleFor(x => x.IdChuyenKhoa)
            .GreaterThan(0).WithMessage("Vui lòng chọn chuyên khoa hợp lệ.");

        RuleFor(x => x.MatKhau)
            .NotEmpty().WithMessage("Mật khẩu không được để trống.")
            .MinimumLength(6).WithMessage("Mật khẩu phải có ít nhất 6 ký tự.");


    }
}
