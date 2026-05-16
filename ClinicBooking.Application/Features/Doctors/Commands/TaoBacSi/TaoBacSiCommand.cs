using ClinicBooking.Domain.Enums;
using MediatR;

namespace ClinicBooking.Application.Features.Doctors.Commands.TaoBacSi;

public sealed record TaoBacSiCommand(
    string TenDangNhap,
    string Email,
    string SoDienThoai,
    string MatKhau,
    string HoTen,
    int IdChuyenKhoa,
    LoaiHopDong LoaiHopDong,
    string? BangCap,
    int? NamKinhNghiem,
    string? TieuSu) : IRequest<int>;
