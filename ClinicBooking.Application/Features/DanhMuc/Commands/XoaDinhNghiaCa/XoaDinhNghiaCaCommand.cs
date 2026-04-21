using MediatR;

namespace ClinicBooking.Application.Features.DanhMuc.Commands.XoaDinhNghiaCa;

public sealed record XoaDinhNghiaCaCommand(int IdDinhNghiaCa) : IRequest<Unit>;
