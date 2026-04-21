using ClinicBooking.Application.Features.DanhMuc.Dtos;
using MediatR;

namespace ClinicBooking.Application.Features.DanhMuc.Queries.LayPhongById;

public sealed record LayPhongByIdQuery(int IdPhong) : IRequest<PhongResponse>;
