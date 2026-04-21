using ClinicBooking.Application.Features.ToaThuoc.Dtos;
using MediatR;

namespace ClinicBooking.Application.Features.ToaThuoc.Queries.LayToaTheoHoSoKham;

public sealed record LayToaTheoHoSoKhamQuery(int IdHoSoKham) : IRequest<IReadOnlyList<ToaThuocResponse>>;
