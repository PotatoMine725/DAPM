using ClinicBooking.Application.Features.BacSi.Dtos;
using MediatR;

namespace ClinicBooking.Application.Features.BacSi.Queries.LayHoSoCuaToi;

public sealed record LayHoSoCuaToiQuery() : IRequest<BacSiResponse>;
