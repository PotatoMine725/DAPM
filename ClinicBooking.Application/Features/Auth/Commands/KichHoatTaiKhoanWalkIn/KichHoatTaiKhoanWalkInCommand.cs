using ClinicBooking.Application.Features.Auth.Dtos;
using MediatR;

namespace ClinicBooking.Application.Features.Auth.Commands.KichHoatTaiKhoanWalkIn;

public record KichHoatTaiKhoanWalkInCommand(
    int IdTaiKhoan,
    string MaOtp,
    string? Cccd,
    DateOnly? NgaySinh,
    string? HoTen,
    string TenDangNhap,
    string Email,
    string MatKhau) : IRequest<XacThucResponse>;
