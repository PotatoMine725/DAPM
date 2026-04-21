using ClinicBooking.Application.Features.HoSoKham.Dtos;
using MediatR;

namespace ClinicBooking.Application.Features.HoSoKham.Queries.LayHoSoKhamById;

public sealed record LayHoSoKhamByIdQuery(int IdHoSoKham) : IRequest<HoSoKhamResponse>;
