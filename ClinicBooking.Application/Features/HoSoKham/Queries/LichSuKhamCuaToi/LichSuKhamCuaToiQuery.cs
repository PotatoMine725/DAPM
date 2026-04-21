using ClinicBooking.Application.Features.HoSoKham.Dtos;
using MediatR;

namespace ClinicBooking.Application.Features.HoSoKham.Queries.LichSuKhamCuaToi;

public sealed record LichSuKhamCuaToiQuery(
    int SoTrang = 1,
    int KichThuocTrang = 20) : IRequest<IReadOnlyList<HoSoKhamTomTatResponse>>;
