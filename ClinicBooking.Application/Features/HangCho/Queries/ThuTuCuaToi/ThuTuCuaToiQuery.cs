using ClinicBooking.Application.Features.HangCho.Dtos;
using MediatR;

namespace ClinicBooking.Application.Features.HangCho.Queries.ThuTuCuaToi;

public sealed record ThuTuCuaToiQuery(int IdCaLamViec) : IRequest<ThuTuHangChoResponse>;
