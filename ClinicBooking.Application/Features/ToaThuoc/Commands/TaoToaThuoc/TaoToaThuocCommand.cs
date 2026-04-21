using ClinicBooking.Application.Features.ToaThuoc.Dtos;
using MediatR;

namespace ClinicBooking.Application.Features.ToaThuoc.Commands.TaoToaThuoc;

public sealed record TaoToaThuocCommand(
    int IdHoSoKham,
    IReadOnlyList<ToaThuocChiTietInput> DanhSachThuoc) : IRequest<Unit>;
