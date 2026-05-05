using MediatR;

namespace ClinicBooking.Application.Features.Scheduling.Commands.XoaCaLamViec;

public sealed record XoaCaLamViecCommand(int IdCaLamViec) : IRequest;
