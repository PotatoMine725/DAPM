using MediatR;

namespace ClinicBooking.Application.Features.DanhMuc.Commands.XoaPhong;

public sealed record XoaPhongCommand(int IdPhong) : IRequest<Unit>;
