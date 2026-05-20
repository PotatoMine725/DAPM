using MediatR;

namespace ClinicBooking.Application.Features.Scheduling.Queries.ThongKeDuyetCa;

public sealed record ThongKeDuyetCaQuery(DateOnly TuNgay, DateOnly DenNgay) : IRequest<ThongKeDuyetCaResponse>;

public sealed record ThongKeDuyetCaResponse(int SoChoDuyet, int SoDaDuyet, int SoTuChoi, int SoBacSiHopDong);
