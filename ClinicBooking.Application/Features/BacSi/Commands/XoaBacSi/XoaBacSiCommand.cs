using MediatR;

namespace ClinicBooking.Application.Features.BacSi.Commands.XoaBacSi;

public sealed record XoaBacSiCommand(int IdBacSi) : IRequest;
