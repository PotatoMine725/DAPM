using ClinicBooking.Domain.Enums;
using MediatR;

namespace ClinicBooking.Application.Features.NghiPhep.Queries.DanhSachDonNghiPhep;

public sealed record DanhSachDonNghiPhepQuery(
    TrangThaiDuyetDon? TrangThaiDuyet = null) : IRequest<IReadOnlyList<object>>;
