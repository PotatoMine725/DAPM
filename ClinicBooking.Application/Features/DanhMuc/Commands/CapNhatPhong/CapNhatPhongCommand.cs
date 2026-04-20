using MediatR;

namespace ClinicBooking.Application.Features.DanhMuc.Commands.CapNhatPhong;

public sealed record CapNhatPhongCommand(
    int IdPhong,
    string MaPhong,
    string TenPhong,
    int? SucChua,
    string? TrangBi,
    bool TrangThai) : IRequest<Unit>;
