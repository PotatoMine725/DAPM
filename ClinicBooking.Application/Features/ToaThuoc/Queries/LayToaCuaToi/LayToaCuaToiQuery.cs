using ClinicBooking.Application.Features.ToaThuoc.Dtos;
using MediatR;

namespace ClinicBooking.Application.Features.ToaThuoc.Queries.LayToaCuaToi;

public sealed record LayToaCuaToiQuery(
    int SoTrang = 1,
    int KichThuocTrang = 20) : IRequest<IReadOnlyList<ToaThuocResponse>>;
