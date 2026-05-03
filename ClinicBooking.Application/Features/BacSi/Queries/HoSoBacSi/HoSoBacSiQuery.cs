using ClinicBooking.Application.Features.BacSi.Dtos;
using MediatR;

namespace ClinicBooking.Application.Features.BacSi.Queries.HoSoBacSi;

public sealed record HoSoBacSiQuery(int IdBacSi) : IRequest<BacSiResponse>;
