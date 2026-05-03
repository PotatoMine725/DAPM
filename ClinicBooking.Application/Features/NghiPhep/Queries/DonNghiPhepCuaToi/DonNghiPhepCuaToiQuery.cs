using MediatR;

namespace ClinicBooking.Application.Features.NghiPhep.Queries.DonNghiPhepCuaToi;

public sealed record DonNghiPhepCuaToiQuery : IRequest<IReadOnlyList<object>>;
