using MediatR;

namespace ClinicBooking.Application.Features.LichHen.Queries.LayChiTietLichHenLeTan;

public sealed record LayChiTietLichHenLeTanQuery(int IdLichHen)
    : IRequest<ChiTietLichHenLeTanResponse>;
