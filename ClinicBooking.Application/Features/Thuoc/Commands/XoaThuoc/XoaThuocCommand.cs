using MediatR;

namespace ClinicBooking.Application.Features.Thuoc.Commands.XoaThuoc;

public sealed record XoaThuocCommand(int IdThuoc) : IRequest<Unit>;
