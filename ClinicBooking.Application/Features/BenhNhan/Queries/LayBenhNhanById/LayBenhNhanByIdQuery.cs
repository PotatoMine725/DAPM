using ClinicBooking.Application.Features.BenhNhan.Dtos;
using MediatR;

namespace ClinicBooking.Application.Features.BenhNhan.Queries.LayBenhNhanById;

public sealed record LayBenhNhanByIdQuery(int IdBenhNhan) : IRequest<BenhNhanResponse>;
