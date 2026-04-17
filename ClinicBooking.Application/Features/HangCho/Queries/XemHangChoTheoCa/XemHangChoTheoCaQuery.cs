using ClinicBooking.Application.Features.HangCho.Dtos;
using MediatR;

namespace ClinicBooking.Application.Features.HangCho.Queries.XemHangChoTheoCa;

public record XemHangChoTheoCaQuery(int IdCaLamViec) : IRequest<IReadOnlyList<HangChoResponse>>;
