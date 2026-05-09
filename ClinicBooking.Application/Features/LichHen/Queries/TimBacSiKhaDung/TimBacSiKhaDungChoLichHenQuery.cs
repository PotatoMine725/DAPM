using MediatR;

namespace ClinicBooking.Application.Features.LichHen.Queries.TimBacSiKhaDung;

public sealed record TimBacSiKhaDungChoLichHenQuery(int IdLichHen, string TenBacSi)
    : IRequest<List<BacSiKhaDungItem>>;
