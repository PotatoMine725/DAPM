using MediatR;

namespace ClinicBooking.Application.Features.Thuoc.Commands.CapNhatThuoc;

public sealed record CapNhatThuocCommand(
    int IdThuoc,
    string TenThuoc,
    string? HoatChat,
    string? DonVi,
    string? GhiChu) : IRequest<Unit>;
