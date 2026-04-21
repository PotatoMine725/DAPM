using ClinicBooking.Application.Features.DanhMuc.Dtos;
using MediatR;

namespace ClinicBooking.Application.Features.DanhMuc.Queries.LayDichVuById;

public sealed record LayDichVuByIdQuery(int IdDichVu) : IRequest<DichVuResponse>;
