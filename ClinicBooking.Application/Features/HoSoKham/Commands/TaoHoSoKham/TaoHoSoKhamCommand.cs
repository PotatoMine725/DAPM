using MediatR;

namespace ClinicBooking.Application.Features.HoSoKham.Commands.TaoHoSoKham;

public sealed record TaoHoSoKhamCommand(
    int IdLichHen,
    string? ChanDoan,
    string? KetQuaKham,
    string? GhiChu) : IRequest<int>;
