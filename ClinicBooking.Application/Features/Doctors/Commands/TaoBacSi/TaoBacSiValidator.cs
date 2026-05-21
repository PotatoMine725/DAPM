using ClinicBooking.Application.Abstractions.Persistence;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace ClinicBooking.Application.Features.Doctors.Commands.TaoBacSi;

public sealed class TaoBacSiValidator : AbstractValidator<TaoBacSiCommand>
{
    private readonly IAppDbContext _db;

    public TaoBacSiValidator(IAppDbContext db)
    {
        _db = db;

        RuleFor(x => x.TenDangNhap)
            .NotEmpty().WithMessage("Tên đăng nhập không được để trống")
            .MinimumLength(3).WithMessage("Tên đăng nhập phải có ít nhất 3 ký tự")
            .MaximumLength(50).WithMessage("Tên đăng nhập không được vượt quá 50 ký tự")
            .MustAsync(async (tenDangNhap, cancellation) =>
            {
                var trimmed = tenDangNhap.Trim();
                return !await _db.TaiKhoan.AnyAsync(x => x.TenDangNhap == trimmed, cancellation);
            }).WithMessage("Tên đăng nhập đã tồn tại");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email không được để trống")
            .EmailAddress().WithMessage("Email không hợp lệ")
            .MaximumLength(100).WithMessage("Email không được vượt quá 100 ký tự")
            .MustAsync(async (email, cancellation) =>
            {
                var trimmed = email.Trim();
                return !await _db.TaiKhoan.AnyAsync(x => x.Email == trimmed, cancellation);
            }).WithMessage("Email đã được sử dụng");

        RuleFor(x => x.SoDienThoai)
            .NotEmpty().WithMessage("Số điện thoại không được để trống")
            .Matches(@"^0\d{9}$").WithMessage("Số điện thoại phải có 10 chữ số và bắt đầu bằng 0")
            .MustAsync(async (sdt, cancellation) =>
            {
                var trimmed = sdt.Trim();
                return !await _db.TaiKhoan.AnyAsync(x => x.SoDienThoai == trimmed, cancellation);
            }).WithMessage("Số điện thoại đã được sử dụng");

        RuleFor(x => x.MatKhau)
            .NotEmpty().WithMessage("Mật khẩu không được để trống")
            .MinimumLength(8).WithMessage("Mật khẩu phải có ít nhất 8 ký tự")
            .Must(password => Regex.IsMatch(password, @"[A-Z]")).WithMessage("Mật khẩu phải chứa ít nhất 1 chữ hoa")
            .Must(password => Regex.IsMatch(password, @"[a-z]")).WithMessage("Mật khẩu phải chứa ít nhất 1 chữ thường")
            .Must(password => Regex.IsMatch(password, @"\d")).WithMessage("Mật khẩu phải chứa ít nhất 1 chữ số");

        RuleFor(x => x.HoTen)
            .NotEmpty().WithMessage("Họ tên không được để trống")
            .MinimumLength(3).WithMessage("Họ tên phải có ít nhất 3 ký tự")
            .MaximumLength(100).WithMessage("Họ tên không được vượt quá 100 ký tự");

        RuleFor(x => x.IdChuyenKhoa)
            .GreaterThan(0).WithMessage("Vui lòng chọn chuyên khoa hợp lệ")
            .MustAsync(async (idChuyenKhoa, cancellation) =>
            {
                return await _db.ChuyenKhoa.AnyAsync(x => x.IdChuyenKhoa == idChuyenKhoa, cancellation);
            }).WithMessage("Không tìm thấy chuyên khoa");

        RuleFor(x => x.BangCap)
            .MaximumLength(200).WithMessage("Bằng cấp không được vượt quá 200 ký tự")
            .When(x => !string.IsNullOrWhiteSpace(x.BangCap));

        RuleFor(x => x.NamKinhNghiem)
            .GreaterThanOrEqualTo(0).WithMessage("Năm kinh nghiệm phải lớn hơn hoặc bằng 0")
            .LessThanOrEqualTo(50).WithMessage("Năm kinh nghiệm không được vượt quá 50")
            .When(x => x.NamKinhNghiem.HasValue);

        RuleFor(x => x.TieuSu)
            .MaximumLength(1000).WithMessage("Tiểu sử không được vượt quá 1000 ký tự")
            .When(x => !string.IsNullOrWhiteSpace(x.TieuSu));
    }
}
