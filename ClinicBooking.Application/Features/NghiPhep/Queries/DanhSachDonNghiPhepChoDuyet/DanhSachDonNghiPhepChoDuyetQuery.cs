using ClinicBooking.Application.Features.NghiPhep.Dtos;
using MediatR;

namespace ClinicBooking.Application.Features.NghiPhep.Queries.DanhSachDonNghiPhepChoDuyet;

public sealed record DanhSachDonNghiPhepChoDuyetQuery() : IRequest<IReadOnlyList<DonNghiPhepResponse>>;
