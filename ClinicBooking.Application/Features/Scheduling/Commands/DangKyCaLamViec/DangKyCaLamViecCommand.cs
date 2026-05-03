using MediatR;

namespace ClinicBooking.Application.Features.Scheduling.Commands.DangKyCaLamViec;

public sealed record DangKyCaLamViecCommand(
    int IdCaLamViec) : IRequest<int>;
