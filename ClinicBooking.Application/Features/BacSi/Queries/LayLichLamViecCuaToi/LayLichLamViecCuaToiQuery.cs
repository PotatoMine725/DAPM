using ClinicBooking.Application.Features.Scheduling.Dtos;
using MediatR;

namespace ClinicBooking.Application.Features.BacSi.Queries.LayLichLamViecCuaToi;

public sealed record LayLichLamViecCuaToiQuery(
    DateOnly TuNgay,
    DateOnly DenNgay) : IRequest<IReadOnlyList<CaLamViecPublicResponse>>;
