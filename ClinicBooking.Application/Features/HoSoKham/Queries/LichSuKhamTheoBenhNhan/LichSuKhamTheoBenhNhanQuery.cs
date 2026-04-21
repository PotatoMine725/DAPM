using ClinicBooking.Application.Features.HoSoKham.Dtos;
using MediatR;

namespace ClinicBooking.Application.Features.HoSoKham.Queries.LichSuKhamTheoBenhNhan;

public sealed record LichSuKhamTheoBenhNhanQuery(
    int IdBenhNhan,
    int SoTrang = 1,
    int KichThuocTrang = 20) : IRequest<IReadOnlyList<HoSoKhamTomTatResponse>>;
