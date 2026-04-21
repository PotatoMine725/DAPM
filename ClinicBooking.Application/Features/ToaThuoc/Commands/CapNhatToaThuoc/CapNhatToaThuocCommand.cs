using ClinicBooking.Application.Features.ToaThuoc.Dtos;
using MediatR;

namespace ClinicBooking.Application.Features.ToaThuoc.Commands.CapNhatToaThuoc;

public sealed record CapNhatToaThuocCommand(
    int IdHoSoKham,
    IReadOnlyList<ToaThuocChiTietInput> DanhSachThuoc) : IRequest<Unit>;
