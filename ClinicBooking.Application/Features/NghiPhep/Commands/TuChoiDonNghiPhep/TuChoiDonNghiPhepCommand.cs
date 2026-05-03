using MediatR;

namespace ClinicBooking.Application.Features.NghiPhep.Commands.TuChoiDonNghiPhep;

public sealed record TuChoiDonNghiPhepCommand(int IdDonNghiPhep, int IdNguoiDuyet, string LyDoTuChoi) : IRequest;
