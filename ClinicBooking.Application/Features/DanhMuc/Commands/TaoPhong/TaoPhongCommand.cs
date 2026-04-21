using MediatR;

namespace ClinicBooking.Application.Features.DanhMuc.Commands.TaoPhong;

public sealed record TaoPhongCommand(
    string MaPhong,
    string TenPhong,
    int? SucChua,
    string? TrangBi,
    bool TrangThai) : IRequest<int>;
