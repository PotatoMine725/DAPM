using MediatR;

namespace ClinicBooking.Application.Features.DanhMuc.Commands.XoaDichVu;

public sealed record XoaDichVuCommand(int IdDichVu) : IRequest<Unit>;
