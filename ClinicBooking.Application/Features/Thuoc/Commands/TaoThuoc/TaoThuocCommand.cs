using MediatR;

namespace ClinicBooking.Application.Features.Thuoc.Commands.TaoThuoc;

public sealed record TaoThuocCommand(
    string TenThuoc,
    string? HoatChat,
    string? DonVi,
    string? GhiChu) : IRequest<int>;
