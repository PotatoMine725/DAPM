using ClinicBooking.Application.Features.Thuoc.Dtos;
using MediatR;

namespace ClinicBooking.Application.Features.Thuoc.Queries.LayThuocById;

public sealed record LayThuocByIdQuery(int IdThuoc) : IRequest<ThuocResponse>;
