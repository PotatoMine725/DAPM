using MediatR;

namespace ClinicBooking.Application.Features.BacSi.Commands.TaoBacSi;

public sealed record TaoBacSiCommand(
    int IdTaiKhoan,
    int IdChuyenKhoa,
    string HoTen,
    string? AnhDaiDien,
    string? BangCap,
    int? NamKinhNghiem,
    string? TieuSu,
    string LoaiHopDong,
    string TrangThai) : IRequest<int>;
