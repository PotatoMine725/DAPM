using MediatR;

namespace ClinicBooking.Application.Features.HoSoKham.Commands.CapNhatHoSoKham;

public sealed record CapNhatHoSoKhamCommand(
    int IdHoSoKham,
    string? ChanDoan,
    string? KetQuaKham,
    string? GhiChu) : IRequest<Unit>;
