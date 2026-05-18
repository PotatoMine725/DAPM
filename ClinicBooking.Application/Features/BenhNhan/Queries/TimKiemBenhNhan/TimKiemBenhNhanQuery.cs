using ClinicBooking.Application.Features.BenhNhan.Dtos;
using MediatR;

namespace ClinicBooking.Application.Features.BenhNhan.Queries.TimKiemBenhNhan;

public sealed record TimKiemBenhNhanQuery(string TuKhoa, int Limit = 20)
    : IRequest<IReadOnlyList<BenhNhanResponse>>;
