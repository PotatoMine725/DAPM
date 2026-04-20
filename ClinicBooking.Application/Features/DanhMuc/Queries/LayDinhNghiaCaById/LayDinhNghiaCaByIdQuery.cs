using ClinicBooking.Application.Features.DanhMuc.Dtos;
using MediatR;

namespace ClinicBooking.Application.Features.DanhMuc.Queries.LayDinhNghiaCaById;

public sealed record LayDinhNghiaCaByIdQuery(int IdDinhNghiaCa) : IRequest<DinhNghiaCaResponse>;
