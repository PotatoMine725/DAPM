using ClinicBooking.Application.Features.Scheduling.Dtos;
using MediatR;

namespace ClinicBooking.Application.Features.Scheduling.Queries.LayChiTietCaLamViecCongKhai;

public sealed record LayChiTietCaLamViecCongKhaiQuery(int IdCaLamViec) : IRequest<CaLamViecPublicResponse?>;