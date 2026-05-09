using MediatR;

namespace ClinicBooking.Application.Features.LichHen.Commands.GanBacSiChoLichHen;

public sealed record GanBacSiChoLichHenCommand(int IdLichHen, int IdCaLamViecMoi)
    : IRequest<GanBacSiKetQua>;
