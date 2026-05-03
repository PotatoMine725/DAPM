using MediatR;

namespace ClinicBooking.Application.Features.Scheduling.Commands.DuyetCaLamViec;

public sealed record DuyetCaLamViecCommand(
    int IdCaLamViec,
    bool ChapNhan,
    string? LyDoTuChoi,
    int IdAdminDuyet) : IRequest;
