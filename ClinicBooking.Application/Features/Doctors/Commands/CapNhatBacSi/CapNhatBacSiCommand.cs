using ClinicBooking.Domain.Enums;
using MediatR;

namespace ClinicBooking.Application.Features.Doctors.Commands.CapNhatBacSi;

public sealed record CapNhatBacSiCommand(
    int IdBacSi,
    string HoTen,
    int IdChuyenKhoa,
    LoaiHopDong LoaiHopDong,
    TrangThaiBacSi TrangThai,
    string? BangCap,
    int? NamKinhNghiem,
    string? TieuSu,
    string? Email,
    string? SoDienThoai) : IRequest;
