using ClinicBooking.Application.Features.Auth.Dtos;
using ClinicBooking.Domain.Enums;
using MediatR;

namespace ClinicBooking.Application.Features.Auth.Commands.DangKy;

public record DangKyCommand(
    string TenDangNhap,
    string Email,
    string SoDienThoai,
    string MatKhau,
    string HoTen,
    DateOnly? NgaySinh,
    GioiTinh? GioiTinh,
    string? Cccd,
    string? DiaChi) : IRequest<XacThucResponse>;
