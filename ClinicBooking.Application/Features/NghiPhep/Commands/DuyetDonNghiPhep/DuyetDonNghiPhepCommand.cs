using MediatR;

namespace ClinicBooking.Application.Features.NghiPhep.Commands.DuyetDonNghiPhep;

public sealed record DuyetDonNghiPhepCommand(int IdDonNghiPhep, int IdNguoiDuyet) : IRequest;
