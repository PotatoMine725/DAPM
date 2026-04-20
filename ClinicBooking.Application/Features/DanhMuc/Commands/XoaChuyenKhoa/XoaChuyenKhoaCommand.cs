using MediatR;

namespace ClinicBooking.Application.Features.DanhMuc.Commands.XoaChuyenKhoa;

public sealed record XoaChuyenKhoaCommand(int IdChuyenKhoa) : IRequest<Unit>;
