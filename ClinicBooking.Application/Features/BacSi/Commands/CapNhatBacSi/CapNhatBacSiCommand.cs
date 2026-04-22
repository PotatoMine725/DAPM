using MediatR;

namespace ClinicBooking.Application.Features.BacSi.Commands.CapNhatBacSi;

public sealed record CapNhatBacSiCommand(
    int IdBacSi,
    int IdChuyenKhoa,
    string HoTen,
    string? AnhDaiDien,
    string? BangCap,
    int? NamKinhNghiem,
    string? TieuSu,
    string LoaiHopDong,
    string TrangThai) : IRequest;
