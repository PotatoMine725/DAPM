using ClinicBooking.Application.Features.Doctors.Dtos;
using MediatR;

namespace ClinicBooking.Application.Features.Doctors.Queries.LayHoSoBacSiCuaToi;

public sealed record LayHoSoBacSiCuaToiQuery : IRequest<HoSoBacSiCuaToiResponse?>;
