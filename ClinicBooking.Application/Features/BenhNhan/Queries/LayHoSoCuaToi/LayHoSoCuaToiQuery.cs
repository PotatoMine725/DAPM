using ClinicBooking.Application.Features.BenhNhan.Dtos;
using MediatR;

namespace ClinicBooking.Application.Features.BenhNhan.Queries.LayHoSoCuaToi;

public sealed record LayHoSoCuaToiQuery : IRequest<BenhNhanResponse>;
