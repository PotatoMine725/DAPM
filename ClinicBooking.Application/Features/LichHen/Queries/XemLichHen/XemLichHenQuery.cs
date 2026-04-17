using ClinicBooking.Application.Features.LichHen.Dtos;
using MediatR;

namespace ClinicBooking.Application.Features.LichHen.Queries.XemLichHen;

public record XemLichHenQuery(int IdLichHen) : IRequest<LichHenResponse>;
