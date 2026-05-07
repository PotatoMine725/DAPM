using ClinicBooking.Application.Features.HoSoKham.Dtos;
using MediatR;

namespace ClinicBooking.Application.Features.HoSoKham.Queries.LichSuHoSoKhamCuaBacSi;

public record LichSuHoSoKhamCuaBacSiQuery(int IdBacSi, int SoTrang = 1, int KichThuocTrang = 20) 
    : IRequest<IReadOnlyList<HoSoKhamTomTatResponse>>;
